using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerSkillGridVM : ViewModel
{
    MBBindingList<TemplateManagerSkillVM> _skillsVM;

    private TemplateManagerCharacter _templateCharacter;
    public const int MAX_SKILL_LEVEL = 330;
    
    private TemplateManagerSkillVM _currentSkillVM;
    
    public TemplateManagerSkillGridVM(TemplateManagerCharacter templateCharacter )
    {
        _templateCharacter = templateCharacter;
        _skillsVM = new MBBindingList<TemplateManagerSkillVM>();
        RefreshHeroSkills();
    }

    [DataSourceProperty]
    public MBBindingList<TemplateManagerSkillVM> SkillsVM
    {
        get
        {
            return _skillsVM;
        }
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
        get
        {
            return _currentSkillVM;
        }
        set
        {
            if (value != _currentSkillVM)
            {
                _currentSkillVM = value;
                OnPropertyChangedWithValue(value, "CurrentSkill");
            }
        }
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
        
    }

    private void ExecuteSaveTemplate()
    {
        
    }
    
    
    private void RefreshHeroSkills()
    {
        SkillsVM.Clear();
        var skillObjectList = CharacterUtils.GetSkillsWithWarSails();
        foreach (SkillObject current in skillObjectList)
        {
            SkillsVM.Add(new TemplateManagerSkillVM(current, _templateCharacter, OnSkillSelectedChange));
        }
        SkillsVM[0].IsInspected = true;
        _currentSkillVM = SkillsVM[0];
        OnPropertyChanged("CurrentSkill");
    }
    
    public void OnSkillSelectedChange(TemplateManagerSkillVM templateManagerSkillVM)
    {
        if (_currentSkillVM != null)
        {
            _currentSkillVM.IsInspected = false;
        }
        CurrentSkill = templateManagerSkillVM;
    }
}