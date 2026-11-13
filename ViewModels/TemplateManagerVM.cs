using System.Collections.Generic;
using HeroPathAutoBuildAssignment.Models;
using HeroPathAutoBuildAssignment.State;
using HeroPathAutoBuildAssignment.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace HeroPathAutoBuildAssignment.ViewModels;

public class TemplateManagerVM : ViewModel
{
    private TemplateManagerCharacter _currentTemplate;
    private int _currentTemplateIndex;
    private string _cancelLbl;
    private string _doneLbl;
    private List<TemplateManagerCharacter> _templateManagerCharacterList;
    TemplateManagerSkillGridVM _currentSkillGridVM;
    private MBBindingList<TemplateManagerSkillGridVM> SkillGridListVm;
    private TemplateListVM _templateListVM;

    public TemplateManagerVM(TemplateManagerState templateManagerState)
    {
        templateManagerState.EditTemplate = new TemplateManagerCharacter();
        templateManagerState.EditTemplate.CreateNewTemplate();
        _templateManagerCharacterList = new List<TemplateManagerCharacter>();
        _templateManagerCharacterList = TemplateSaveManager.LoadSavedTemplatesList();
        int selectedIndex = 0;
        if (_templateManagerCharacterList.Count > 0)
        {
            selectedIndex =
                TemplateHeroUtils.FindTemplateIndexForHero(_templateManagerCharacterList,
                    templateManagerState.SelectedHero);
            _currentTemplate = _templateManagerCharacterList[selectedIndex];
        }
        else
        {
            var newTemplate = new TemplateManagerCharacter
            {
                Name = "New created template"
            };
            newTemplate.CreateNewTemplate();
            _templateManagerCharacterList.Add(newTemplate);
            _currentTemplate = _templateManagerCharacterList[0];
        }

        SkillGridListVm = new MBBindingList<TemplateManagerSkillGridVM>();

        foreach (var templateManagerCharacter in _templateManagerCharacterList)
        {
            SkillGridListVm.Add(new TemplateManagerSkillGridVM(templateManagerCharacter, OnSaveTemplate));
        }

        _currentTemplateIndex = selectedIndex;
        _currentSkillGridVM = SkillGridListVm[selectedIndex];
        _templateListVM = new TemplateListVM(_templateManagerCharacterList, OnTemplateCharacterSelectedChange,
            OnCreateNewTemplate, OnDeleteTemplate, selectedIndex);

        _cancelLbl = new TextObject("{=3CpNUnVl}Cancel").ToString();
        _doneLbl = new TextObject("{=WiNRdfsm}Done").ToString();
        HeroesAssignedAtOpen = CollectAssignedHeroIds(_templateManagerCharacterList);
        RefreshValues();
    }

    public HashSet<string> HeroesAssignedAtOpen { get; }

    public static HashSet<string> CollectAssignedHeroIds(List<TemplateManagerCharacter> templates)
    {
        var heroIds = new HashSet<string>();
        foreach (var template in templates)
        {
            if (template?.HeroList == null)
            {
                continue;
            }

            foreach (var hero in template.HeroList)
            {
                if (hero != null && !string.IsNullOrEmpty(hero.StringId))
                {
                    heroIds.Add(hero.StringId);
                }
            }
        }

        return heroIds;
    }

    public void ExecuteCancel()
    {
        var index = 0;
        bool unSavedTemplate = false;
        foreach (var listItemVm in _templateListVM.ListItemVM)
        {
            if (listItemVm._templateManagerCharacter.GetIsFromNewCreatedTemplate() ||
                SkillGridListVm[index].TemplateCharacter.HasUnsavedChanges())
            {
                unSavedTemplate = true;
                break;
            }

            index++;
        }

        if (unSavedTemplate)
        {
            string unsavedInquiryTitle = new TextObject("{=unsaved_inquiry_title}There is some unsaved template").ToString();
            string unsavedInquiryText = new TextObject("{=unsaved_inquiry_text}Are you sure you want to continue?").ToString();
            
            var inquiry = new InquiryData(
                titleText: unsavedInquiryTitle,
                text: unsavedInquiryText,
                isAffirmativeOptionShown: true,
                isNegativeOptionShown: true,
                affirmativeText: "Continue",
                negativeText: "Cancel",
                affirmativeAction: Close,
                negativeAction: InformationManager.HideInquiry
            );
            InformationManager.ShowInquiry(inquiry);
        }
        else
        {
            Close();
        }
    }

    public void ExecuteDone()
    {
        foreach (var skillGridVm in SkillGridListVm)
        {
            bool stillInList = _templateManagerCharacterList.Contains(skillGridVm.TemplateCharacter);
            if (stillInList)
            {
                skillGridVm.SaveUsingCurrentName();
            }
        }

        Close();
    }

    public void ExecuteReset()
    {
        foreach (var skillGridVm in SkillGridListVm)
        {
            if (skillGridVm.TemplateCharacter.GetIsFromNewCreatedTemplate())
            {
                var result =
                    _templateListVM._templateManagerCharacterList.Find(obj =>
                        obj.Equals(skillGridVm.TemplateCharacter));
                _templateListVM._templateManagerCharacterList.Remove(result);
                continue;
            }

            if (skillGridVm.TemplateCharacter.HasUnsavedChanges())
            {
                var dto = skillGridVm.TemplateCharacter.SavedStateSnapshot;
                foreach (var skill in CharacterUtils.GetSkillsWithWarSails())
                {
                    skillGridVm.TemplateCharacter.SetImportantSkill(skill, false);
                    var skillVmIndex = skillGridVm.SkillsVM.FindIndex(obj => obj.SkillId.Equals(skill.StringId));
                    skillGridVm.SkillsVM[skillVmIndex].IsImportantSkill = false;
                }

                skillGridVm.TemplateCharacter.ClearAllPerks();
                dto.ApplyToModel(skillGridVm.TemplateCharacter);
                foreach (var importantSkillId in dto.ImportantSkillIdList)
                {
                    var skillVmIndex = skillGridVm.SkillsVM.FindIndex(obj => obj.SkillId.Equals(importantSkillId));
                    skillGridVm.SkillsVM[skillVmIndex].IsImportantSkill = true;
                }

                foreach (var skillsVM in skillGridVm.SkillsVM)
                {
                    skillsVM.RefreshSkillPerksState();
                }
            }
        }

        _templateListVM.RefreshTemplateList(0);
        _currentSkillGridVM = SkillGridListVm[0];
        OnPropertyChanged("TemplateSkillGridVM");
    }

    private void Close()
    {
        GameStateManager.Current.PopState();
    }

    private void OnTemplateCharacterSelectedChange(TemplateManagerCharacter templateManagerCharacter, int index)
    {
        _currentTemplate = templateManagerCharacter;
        _currentSkillGridVM = SkillGridListVm[index];
        _currentTemplateIndex = index;
        OnPropertyChanged("TemplateSkillGridVM");
    }

    private void OnCreateNewTemplate(TemplateManagerCharacter newTemplate)
    {
        newTemplate.SetIsFromNewCreatedTemplate(true);
        SkillGridListVm.Add(new TemplateManagerSkillGridVM(newTemplate, OnSaveTemplate));
        _currentTemplate = newTemplate;
        _currentSkillGridVM = SkillGridListVm[SkillGridListVm.Count - 1];
        _currentTemplateIndex = _templateListVM.ListItemVM.Count - 1;
        OnPropertyChanged("TemplateSkillGridVM");
    }

    private void OnDeleteTemplate(TemplateManagerCharacter deletedTemplate)
    {
        for (int i = 0; i < SkillGridListVm.Count; i++)
        {
            if (ReferenceEquals(SkillGridListVm[i].TemplateCharacter, deletedTemplate))
            {
                SkillGridListVm.RemoveAt(i);
                break;
            }
        }

        int lastIndex = _templateManagerCharacterList.Count - 1;
        if (lastIndex < 0 || lastIndex >= SkillGridListVm.Count)
        {
            return;
        }

        _currentTemplate = _templateManagerCharacterList[lastIndex];
        _currentSkillGridVM = SkillGridListVm[lastIndex];
        _currentTemplateIndex = lastIndex;
        OnPropertyChanged("TemplateSkillGridVM");
    }

    private void OnSaveTemplate(TemplateManagerCharacter templateCharacter)
    {
        templateCharacter.SetIsFromNewCreatedTemplate(false);
        int index = _templateManagerCharacterList.FindIndex(template => ReferenceEquals(template, templateCharacter));
        if (index < 0)
        {
            return;
        }

        _currentTemplate = templateCharacter;
        _currentTemplateIndex = index;
        _templateListVM.RefreshTemplateList(index);
    }

    private void RefreshValues()
    {
        OnPropertyChanged(nameof(CancelLbl));
        OnPropertyChanged(nameof(DoneLbl));
        OnPropertyChanged(nameof(TemplateListVM));
        OnPropertyChanged(nameof(TemplateSkillGridVM));
    }

    [DataSourceProperty]
    public string CancelLbl
    {
        get => _cancelLbl;
        set
        {
            if (_cancelLbl != value)
            {
                _cancelLbl = value;
                OnPropertyChanged(nameof(CancelLbl));
            }
        }
    }

    [DataSourceProperty]
    public string DoneLbl
    {
        get => _doneLbl;
        set
        {
            if (_doneLbl != value)
            {
                _doneLbl = value;
                OnPropertyChanged(nameof(DoneLbl));
            }
        }
    }

    [DataSourceProperty]
    public TemplateManagerSkillGridVM TemplateSkillGridVM
    {
        get { return _currentSkillGridVM; }
        set
        {
            if (_currentSkillGridVM != value)
            {
                _currentSkillGridVM = value;
                OnPropertyChangedWithValue(value, "TemplateSkillGridVM");
            }
        }
    }

    [DataSourceProperty]
    public TemplateListVM TemplateListVM
    {
        get { return _templateListVM; }
    }

    [DataSourceProperty]
    public string TitleTemplateText { get; set; } = new TextObject("{=templates_title}Templates").ToString();
}