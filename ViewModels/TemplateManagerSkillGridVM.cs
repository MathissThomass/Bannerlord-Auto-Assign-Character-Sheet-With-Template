using System;
using System.Text.RegularExpressions;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Path = System.IO.Path;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerSkillGridVM : ViewModel
{
    MBBindingList<TemplateManagerSkillVM> _skillsVM;

    private TemplateManagerCharacter _templateCharacter;

    private TemplateManagerSkillVM _currentSkillVM;

    private bool _isImportantSkillToggleOn;

    public TemplateManagerSkillGridVM(TemplateManagerCharacter templateCharacter)
    {
        _templateCharacter = templateCharacter;
        _skillsVM = new MBBindingList<TemplateManagerSkillVM>();
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
        _templateCharacter.ClearPerkSkill(_currentSkillVM.Skill);
        _currentSkillVM.RefreshSkillPerksState();
    }

    private void ExecuteClearAllPerks()
    {
        _templateCharacter.ClearAllPerks();
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
        InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("Enter the template name").ToString(),
            new TextObject("Can only save up to 15").ToString(),
            true, true, GameTexts.FindText("str_done", null).ToString(),
            GameTexts.FindText("str_cancel", null).ToString(), OnEnterNameAfter, InformationManager.HideInquiry,
            false));
    }

    private void OnEnterNameAfter(string saveName)
    {
        _templateCharacter.Name = saveName;
        var data = TemplateCharacterDto.FromModel(_templateCharacter);
        TemplateSaveManager.SaveTemplate(data);
    }


    private void RefreshHeroSkills()
    {
        SkillsVM.Clear();
        var skillObjectList = CharacterUtils.GetSkillsWithWarSails();
        foreach (SkillObject current in skillObjectList)
        {
            SkillsVM.Add(new TemplateManagerSkillVM(current, _templateCharacter,
                _templateCharacter.GetImportantSkill(current), OnSkillSelectedChange));
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
            _templateCharacter.SetImportantSkill(_currentSkillVM.Skill, _currentSkillVM.IsImportantSkill);
        }
    }
}