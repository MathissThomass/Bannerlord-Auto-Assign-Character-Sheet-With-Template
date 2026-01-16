using System;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateListItemVM : ViewModel
{
    private bool _isInspected;
    private string _templateName;
    Action<TemplateListItemVM> _onTemplateSelection;
    private MBBindingList<TemplateSkillOverviewVM> _skillsVm;
    public TemplateManagerCharacter _templateManagerCharacter;

    public TemplateListItemVM(TemplateManagerCharacter templateManagerCharacter,
        Action<TemplateListItemVM> onTemplateSelection)
    {
        _isInspected = false;
        _templateName = templateManagerCharacter.Name;
        _templateManagerCharacter = templateManagerCharacter;
        _onTemplateSelection = onTemplateSelection;
        _skillsVm = new MBBindingList<TemplateSkillOverviewVM>();
        RefreshSkillsVm(templateManagerCharacter);
    }

    private void OnTemplateSelected()
    {
        IsInspected = true;
        _onTemplateSelection(this);
    }

    public void RefreshSkillsVm(TemplateManagerCharacter templateManagerCharacter)
    {
        SkillsVm.Clear();

        var skillList = CharacterUtils.GetSkillsWithWarSails();
        foreach (var skill in skillList)
        {
            var isImportantSkill = templateManagerCharacter.GetImportantSkill(skill);
            SkillsVm.Add(new TemplateSkillOverviewVM(skill, isImportantSkill));
        }

        OnPropertyChanged("SkillsVm");
    }

    [DataSourceProperty]
    public string TemplateName
    {
        get { return _templateName; }
        set
        {
            if (_templateName != value)
            {
                _templateName = value;
                OnPropertyChangedWithValue(value, "TemplateName");
            }
        }
    }

    [DataSourceProperty]
    public bool IsInspected
    {
        get { return _isInspected; }
        set
        {
            if (value != _isInspected)
            {
                _isInspected = value;
                OnPropertyChangedWithValue(value, "IsInspected");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<TemplateSkillOverviewVM> SkillsVm
    {
        get { return _skillsVm; }
        set
        {
            if (_skillsVm != value)
            {
                _skillsVm = value;
                OnPropertyChangedWithValue(value, "SkillsVm");
            }
        }
    }
}