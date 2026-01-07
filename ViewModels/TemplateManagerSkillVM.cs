using System;
using System.Collections.Generic;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerSkillVM : ViewModel
{
    private string _nameText;
    private string _skillId;
    private readonly SkillObject _skillObject;
    private MBBindingList<TemplateManagerPerkVM> _perks;
    private TemplateManagerCharacter _template;
    private bool _isInspected;
    private float _learningRate;
    private int _fullLearningRateLevel;
    private bool _canLearnSkill;
    Action<TemplateManagerSkillVM> onSkillSelection;


    public TemplateManagerSkillVM(SkillObject skillObject, TemplateManagerCharacter template,
        Action<TemplateManagerSkillVM> onSkillSelection)
    {
        _perks = new MBBindingList<TemplateManagerPerkVM>();
        _skillObject = skillObject;
        SkillId = skillObject.StringId;
        NameText = skillObject.Name.ToString();
        FillHeroData(template);
        this.onSkillSelection = onSkillSelection;
    }

    public SkillObject Skill
    {
        get { return _skillObject; }
    }

    public void FillHeroData(TemplateManagerCharacter template)
    {
        _template = template;
        LearningRate = 0;
        CanLearnSkill = true;
        _fullLearningRateLevel = 330;

        RefreshPerks();
    }

    [DataSourceProperty]
    public MBBindingList<TemplateManagerPerkVM> Perks
    {
        get { return _perks; }
        set
        {
            if (value != _perks)
            {
                _perks = value;
                OnPropertyChangedWithValue(value, "Perks");
            }
        }
    }

    [DataSourceProperty]
    public int FullLearningRateLevel
    {
        get { return _fullLearningRateLevel; }
        set
        {
            if (value != _fullLearningRateLevel)
            {
                _fullLearningRateLevel = value;
                OnPropertyChangedWithValue(value, "FullLearningRateLevel");
            }
        }
    }
    

    [DataSourceProperty]
    public bool CanLearnSkill
    {
        get { return _canLearnSkill; }
        set
        {
            if (value != _canLearnSkill)
            {
                _canLearnSkill = value;
                OnPropertyChangedWithValue(value, "CanLearnSkill");
            }
        }
    }

    [DataSourceProperty]
    public float LearningRate
    {
        get { return _learningRate; }
        set
        {
            if (value != _learningRate)
            {
                _learningRate = value;
                OnPropertyChangedWithValue(value, "LearningRate");
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
    public string NameText
    {
        get { return _nameText; }
        set
        {
            if (value != _nameText)
            {
                _nameText = value;
                OnPropertyChangedWithValue(value, "NameText");
            }
        }
    }

    [DataSourceProperty]
    public string SkillId
    {
        get { return _skillId; }
        set
        {
            if (value != _skillId)
            {
                _skillId = value;
                OnPropertyChangedWithValue(value, "SkillId");
            }
        }
    }

    private void ExecuteInspect()
    {
        IsInspected = true;
        onSkillSelection(this);
    }



    private void RefreshPerks()
    {
        Perks.Clear();
        PerkObject? perkAlternative = null;

        var listPerks = PerkObject.All
            .Where(p => p.Skill == Skill)
            .OrderBy(p => p.RequiredSkillValue);

        foreach (var perk in listPerks)
        {
            if (perk.AlternativePerk != null)
            {
                var altType = (perkAlternative == null)
                    ? TemplateManagerPerkVM.PerkAlternativeType.FirstAlternative
                    : TemplateManagerPerkVM.PerkAlternativeType.SecondAlternative;
                

                var vm = new TemplateManagerPerkVM(perk, IsPerkSelected(perk), altType, OnPerkSelectedChange);
                Perks.Add(vm);

                if (altType == TemplateManagerPerkVM.PerkAlternativeType.SecondAlternative)
                    perkAlternative = null;
                else
                    perkAlternative = perk.AlternativePerk;
            }
            else
            {
                var vm = new TemplateManagerPerkVM(perk, IsPerkSelected(perk),
                    TemplateManagerPerkVM.PerkAlternativeType.NoAlternative, OnPerkSelectedChange);
                Perks.Add(vm);
                perkAlternative = null;
            }
        }
    }

    private void OnPerkSelectedChange(PerkObject perk, bool selected)
    {
        _template.SetPerkValue(perk, selected);
    }

    private bool IsPerkSelected(PerkObject perk)
    {
        return _template.GetPerkValue(perk);
    }
}