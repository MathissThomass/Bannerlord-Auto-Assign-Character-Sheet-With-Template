using System;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerSkillGridVM : ViewModel
{
    MBBindingList<TemplateManagerSkillVM> _skillsVM;

    public TemplateManagerCharacter TemplateCharacter;

    private TemplateManagerSkillVM _currentSkillVM;

    private bool _isImportantSkillToggleOn;
    
    Action<TemplateManagerCharacter> _onSaveTemplate;

    public TemplateManagerSkillGridVM(TemplateManagerCharacter templateCharacter, Action<TemplateManagerCharacter> onSaveTemplate)
    {
        TemplateCharacter = templateCharacter;
        _skillsVM = new MBBindingList<TemplateManagerSkillVM>();
        _onSaveTemplate = onSaveTemplate;
        RefreshHeroSkills();
    }

    [DataSourceProperty]
    public MBBindingList<TemplateManagerSkillVM> SkillsVM
    {
        get { return _skillsVM; }
        set
        {
            if (value != _skillsVM)
            {
                _skillsVM = value;
                OnPropertyChangedWithValue(value, "SkillsVM");
            }
        }
    }

    [DataSourceProperty]
    public TemplateManagerSkillVM CurrentSkill
    {
        get { return _currentSkillVM; }
        set
        {
            if (value != _currentSkillVM)
            {
                _currentSkillVM = value;
                OnPropertyChangedWithValue(value, "CurrentSkill");
            }
        }
    }

    [DataSourceProperty]
    public string IsImportantButtonBrushStringId
    {
        get { return _isImportantSkillToggleOn ? "ButtonBrush1" : "ButtonBrush2"; }
    }

    private void ExecuteClearSkillPerks()
    {
        TemplateCharacter.ClearPerkSkill(_currentSkillVM.Skill);
        _currentSkillVM.RefreshSkillPerksState();
    }

    private void ExecuteClearAllPerks()
    {
        TemplateCharacter.ClearAllPerks();
        foreach (var skillsVM in _skillsVM)
        {
            skillsVM.RefreshSkillPerksState();
        }
    }

    private void ExecuteImportantSkillToggle()
    {
        _isImportantSkillToggleOn = !_isImportantSkillToggleOn;
        OnPropertyChanged("IsImportantButtonBrushStringId");
    }

    private void ExecuteSaveTemplate()
    {
        if (!TemplateCharacter.GetIsFromNewCreatedTemplate())
        {
            SaveTemplate();
        }
        else
        {
            var textInquiry = new TextInquiryData(
                titleText: "Enter the template name",
                text: "Template name :",
                isAffirmativeOptionShown: true,
                isNegativeOptionShown: true,
                affirmativeText: GameTexts.FindText("str_done", null).ToString(),
                negativeText: GameTexts.FindText("str_cancel", null).ToString(),
                affirmativeAction: OnEnterNameAfter,
                negativeAction: InformationManager.HideInquiry,
                textCondition: TemplateSaveManager.CheckIfNameExists
            );
            InformationManager.ShowTextInquiry(textInquiry);
        }
    }

    private void OnEnterNameAfter(string saveName)
    {
        TemplateCharacter.Name = saveName;
        SaveTemplate();
    }

    private void SaveTemplate()
    {
        var data = TemplateCharacterDto.FromModel(TemplateCharacter);
        TemplateSaveManager.SaveTemplate(data);
        _onSaveTemplate(TemplateCharacter);
    }

    private void RefreshHeroSkills()
    {
        SkillsVM.Clear();
        var skillObjectList = CharacterUtils.GetSkillsWithWarSails();
        foreach (SkillObject current in skillObjectList)
        {
            SkillsVM.Add(new TemplateManagerSkillVM(current, TemplateCharacter,
                TemplateCharacter.GetImportantSkill(current), OnSkillSelectedChange));
        }

        SkillsVM[0].IsInspected = true;
        _currentSkillVM = SkillsVM[0];
        OnPropertyChanged("CurrentSkill");
    }
    
    public void OnSkillSelectedChange(TemplateManagerSkillVM templateManagerSkillVM)
    {
        if (templateManagerSkillVM != _currentSkillVM)
        {
            _currentSkillVM.IsInspected = false;
            CurrentSkill = templateManagerSkillVM;
        }

        if (_isImportantSkillToggleOn)
        {
            _currentSkillVM.IsImportantSkill = !_currentSkillVM.IsImportantSkill;
            TemplateCharacter.SetImportantSkill(_currentSkillVM.Skill, _currentSkillVM.IsImportantSkill);
        }
    }
}