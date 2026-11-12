using System;
using System.Collections.Generic;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateListVM : ViewModel
{
    public List<TemplateManagerCharacter> _templateManagerCharacterList;
    MBBindingList<TemplateListItemVM> _listItemVm;
    TemplateListItemVM _currentTemplateListItemVM;
    Action<TemplateManagerCharacter, int> _onTemplateSelectedChanged;
    private Action<TemplateManagerCharacter> _onCreateNewTemplate;
    private Action<TemplateManagerCharacter> _onDeleteTemplate;
    private TemplateHeroSelectionPopupVM _heroSelectionPopupVm;
    private bool _isHeroListOpen;

    public TemplateListVM(List<TemplateManagerCharacter> templateManagerCharacterList,
        Action<TemplateManagerCharacter, int> onTemplateSelectedChanged,
        Action<TemplateManagerCharacter> onCreateNewTemplate,
        Action<TemplateManagerCharacter> onDeleteTemplate)
    {
        _templateManagerCharacterList = templateManagerCharacterList;
        _listItemVm = new MBBindingList<TemplateListItemVM>();
        _onTemplateSelectedChanged = onTemplateSelectedChanged;
        _onCreateNewTemplate = onCreateNewTemplate;
        _onDeleteTemplate = onDeleteTemplate;
        _heroSelectionPopupVm = new TemplateHeroSelectionPopupVM();
        _isHeroListOpen = false;
        RefreshTemplateList(0); // TODO change to the one selected if hero had a temlplate
    }

    public void RefreshTemplateList(int inspectedIndex)
    {
        ListItemVM.Clear();
        foreach (var templateCharacter in _templateManagerCharacterList)
        {
            ListItemVM.Add(new TemplateListItemVM(templateCharacter, OnTemplateSelectedChanged));
        }

        if (ListItemVM.Count > 0)
        {
            ListItemVM[inspectedIndex].IsInspected = true;
            _currentTemplateListItemVM = ListItemVM[inspectedIndex];
        }

        if (IsHeroListOpen)
        {
            _heroSelectionPopupVm.OpenForTemplate(_currentTemplateListItemVM._templateManagerCharacter, _templateManagerCharacterList);
        }

        OnPropertyChanged("ListItemVM");
    }

    private void ExecuteDeleteCurrent()
    {
        string DeleteTemplateInquiryTitle = new TextObject("{=delete_template_inquiry_title}Delete current template").ToString();
        string DeleteTemplateInquiryText = new TextObject("{=delete_template_inquiry_text}Are you sure you want to delete the current template?").ToString();
        string DeleteBtnText = new TextObject("{=deletekey}Delete").ToString();
        string CancelBtnText = new TextObject("{=3CpNUnVl}Cancel").ToString();
        
        var inquiry = new InquiryData(
            titleText: DeleteTemplateInquiryTitle,
            text: DeleteTemplateInquiryText,
            isAffirmativeOptionShown: true,
            isNegativeOptionShown: true,
            affirmativeText: DeleteBtnText,
            negativeText: CancelBtnText,
            affirmativeAction: OnDeleteCurrentTemplate,
            negativeAction: InformationManager.HideInquiry
        );
        InformationManager.ShowInquiry(inquiry);
    }

    private void ExecuteCreateNew()
    {
        string newCreatedTemplateText = new TextObject("{=new_created_template_text}New created template").ToString();
        
        var newTemplate = new TemplateManagerCharacter
        {
            Name = GenerateUniqueName(newCreatedTemplateText)
        };
        newTemplate.CreateNewTemplate();
        _templateManagerCharacterList.Add(newTemplate);
        var index = _templateManagerCharacterList.Count - 1;
        RefreshTemplateList(index);
        _onCreateNewTemplate(newTemplate);
    }

    private void ExecuteDuplicateCurrentTemplate()
    {
        var original = _currentTemplateListItemVM._templateManagerCharacter;
        var json = JsonConvert.SerializeObject(original);
        var newTemplate = JsonConvert.DeserializeObject<TemplateManagerCharacter>(json);
        newTemplate.Name = GenerateUniqueName("Duplicate " + newTemplate.Name);
        newTemplate.CreateNewTemplate();
        _templateManagerCharacterList.Add(newTemplate);
        var index = _templateManagerCharacterList.Count - 1;
        RefreshTemplateList(index);
        _onCreateNewTemplate(newTemplate);
    }

    private void ExecuteRenameCurrentTemplate()
    {
        string NewTemplateInquiryTitle = new TextObject("{=new_template_inquiry_title}Enter the template name").ToString();
        string NewTemplateInquiryText = new TextObject("{=new_template_inquiry_text}New template name :").ToString();
            
        var textInquiry = new TextInquiryData(
            titleText: NewTemplateInquiryTitle,
            text: NewTemplateInquiryText,
            isAffirmativeOptionShown: true,
            isNegativeOptionShown: true,
            affirmativeText: GameTexts.FindText("str_done", null).ToString(),
            negativeText: GameTexts.FindText("str_cancel", null).ToString(),
            affirmativeAction: OnEnterNameAfter,
            negativeAction: InformationManager.HideInquiry,
            textCondition: TemplateSaveManager.CheckIfNameExists,
            defaultInputText: _currentTemplateListItemVM.TemplateName
        );
        InformationManager.ShowTextInquiry(textInquiry);
    }

    private void ExecuteOpenHeroListToAssign()
    {
        _isHeroListOpen = !_isHeroListOpen;
        OnPropertyChanged("IsHeroListOpen");
        OnPropertyChanged("HeroListButtonBrushStringId");
        if (_isHeroListOpen)
        {
            var currentTemplate = _currentTemplateListItemVM._templateManagerCharacter;
            _heroSelectionPopupVm.OpenForTemplate(currentTemplate, _templateManagerCharacterList);
        }
        else
        {
            _heroSelectionPopupVm.Close();
        }
    }

    private void OnEnterNameAfter(string newName)
    {
        if (!_currentTemplateListItemVM._templateManagerCharacter.GetIsFromNewCreatedTemplate())
        {
            TemplateSaveManager.RenameSaveFile(newName, _currentTemplateListItemVM.TemplateName);
        }

        var index = _listItemVm.FindIndex(obj => obj.Equals(_currentTemplateListItemVM));
        _currentTemplateListItemVM.TemplateName = newName;
        _templateManagerCharacterList[index].Name = newName;
        RefreshTemplateList(index);
    }

    public void OnTemplateSelectedChanged(TemplateListItemVM templateListItemVm)
    {
        if (templateListItemVm != _currentTemplateListItemVM)
        {
            if (_currentTemplateListItemVM != null)
            {
                _currentTemplateListItemVM.IsInspected = false;
            }

            CurrentTemplateItemVM = templateListItemVm;
            int index = _listItemVm.IndexOf(templateListItemVm);
            _onTemplateSelectedChanged(CurrentTemplateItemVM._templateManagerCharacter, index);
            _heroSelectionPopupVm.OpenForTemplate(templateListItemVm._templateManagerCharacter, _templateManagerCharacterList);
        }
    }

    private void OnDeleteCurrentTemplate()
    {
        var deletedTemplate = _currentTemplateListItemVM._templateManagerCharacter;
        TemplateSaveManager.DeleteTemplate(deletedTemplate.Name);

        _templateManagerCharacterList.Remove(deletedTemplate);
        _onDeleteTemplate(deletedTemplate);
        int indexToSelect = _templateManagerCharacterList.Count > 0
            ? _templateManagerCharacterList.Count - 1
            : 0;
        RefreshTemplateList(indexToSelect);
    }

    private string GenerateUniqueName(string baseName)
    {
        var existingNames = _templateManagerCharacterList.Select(t => t.Name).ToList();

        if (!existingNames.Contains(baseName))
        {
            return baseName;
        }

        int counter = 2;
        string candidate;

        do
        {
            candidate = $"{baseName} {counter}";
            counter++;
        } while (existingNames.Contains(candidate));
        return candidate;
    }

    [DataSourceProperty]
    public MBBindingList<TemplateListItemVM> ListItemVM
    {
        get { return _listItemVm; }
        set
        {
            if (value != _listItemVm)
            {
                _listItemVm = value;
                OnPropertyChangedWithValue(value, "ListItemVM");
            }
        }
    }

    [DataSourceProperty]
    public TemplateListItemVM CurrentTemplateItemVM
    {
        get { return _currentTemplateListItemVM; }
        set
        {
            if (value != _currentTemplateListItemVM)
            {
                _currentTemplateListItemVM = value;
                OnPropertyChangedWithValue(value, "CurrentTemplateItemVM");
            }
        }
    }
    
    [DataSourceProperty]
    public string HeroListButtonBrushStringId
    {
        get { return _isHeroListOpen ? "ButtonBrush1" : "ButtonBrush2"; }
    }

    [DataSourceProperty]
    public bool IsHeroListOpen
    {
        get { return _isHeroListOpen; }
        set
        {
            if (value != _isHeroListOpen)
            {
                _isHeroListOpen = value;
                OnPropertyChangedWithValue(value, "IsHeroListOpen");
            }
        }
    }
    
    [DataSourceProperty]
    public TemplateHeroSelectionPopupVM HeroSelectionPopupVm
    {
        get { return _heroSelectionPopupVm; }
        set
        {
            if (value != _heroSelectionPopupVm)
            {
                _heroSelectionPopupVm = value;
                OnPropertyChangedWithValue(value, "HeroSelectionPopupVm");
            }
        }
    }
    
    [DataSourceProperty]
    public string TemplateListTitle { get; set; } = new TextObject("{=template_list_title}Template list").ToString();
    
    [DataSourceProperty]
    public string DeleteCurrentText { get; set; } = new TextObject("{=delete_current_text}Delete current").ToString();
    
    [DataSourceProperty]
    public string CreateNewText { get; set; } = new TextObject("{=create_new_text}Create New").ToString();
    
    [DataSourceProperty]
    public string DuplicateCurrentTemplateText { get; set; } = new TextObject("{=duplicate_current_template_text}Duplicate current template").ToString();
    
    [DataSourceProperty]
    public string RenameCurrentTemplateText { get; set; } = new TextObject("{=rename_current_template_text}Rename current template").ToString();
    
    [DataSourceProperty]
    public string AssignToHeroText { get; set; } = new TextObject("{=assign_to_hero_text}Assign to hero").ToString();
    
}