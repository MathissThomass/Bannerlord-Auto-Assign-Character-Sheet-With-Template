using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerPerkVM : ViewModel
{
    public enum PerkStates
    {
        None = -1,
        NotEarned,
        EarnedButNotSelected,
        InSelection,
        EarnedAndActive,
        EarnedAndNotActive,
        EarnedPreviousPerkNotSelected
    }

    public enum PerkAlternativeType
    {
        NoAlternative,
        FirstAlternative,
        SecondAlternative
    }

    public readonly PerkObject Perk;

    private readonly Concept? _perkConceptObj;

    private PerkStates _currentState = PerkStates.None;

    private string _levelText;

    private string _perkId;

    private string _backgroundImage;

    private BasicTooltipViewModel _hint;

    private int _level;

    private int _alternativeType;

    private int _perkState = -1;

    private bool _isTutorialHighlightEnabled;

    Action<PerkObject, bool> _onPerkSelectedChange;
    
    private readonly Func<PerkObject, bool> _getIsPerkSelected;

    public TemplateManagerPerkVM(PerkObject perk, PerkAlternativeType alternativeType, Func<PerkObject, bool> getIsPerkSelected,
        Action<PerkObject, bool> onPerkSelectedChange)
    {
        AlternativeType = (int)alternativeType;
        Perk = perk;
        _onPerkSelectedChange = onPerkSelectedChange;
        PerkId = "SPPerks\\" + perk.StringId;
        Level = (int)perk.RequiredSkillValue;
        LevelText = ((int)perk.RequiredSkillValue).ToString();
        Hint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPerkEffectText(perk, true));
        _perkConceptObj = Concept.All.SingleOrDefault(c => c.StringId == "str_game_objects_perks");
        _getIsPerkSelected =  getIsPerkSelected;
        RefreshState();
    }
    
    public void RefreshState()
    {
        bool isSelected = _getIsPerkSelected(Perk);
        if (isSelected)
        {
            CurrentState = PerkStates.EarnedAndActive;
            return;
        }
        if (Perk.AlternativePerk != null && _getIsPerkSelected(Perk.AlternativePerk))
        {
            CurrentState = PerkStates.EarnedAndNotActive;
            return;
        }
        CurrentState = PerkStates.EarnedButNotSelected;
    }

    public PerkStates CurrentState
    {
        get { return _currentState; }
        private set
        {
            if (value != _currentState)
            {
                _currentState = value;
                PerkState = (int)value;
            }
        }
    }
    
    private bool _hasAlternativeAndSelected
    {
        get
        {
            return AlternativeType != 0 && _getIsPerkSelected(Perk.AlternativePerk);
        }
    }

    [DataSourceProperty]
    public bool IsTutorialHighlightEnabled
    {
        get { return _isTutorialHighlightEnabled; }
        set
        {
            if (value != _isTutorialHighlightEnabled)
            {
                _isTutorialHighlightEnabled = value;
                OnPropertyChangedWithValue(value, "IsTutorialHighlightEnabled");
            }
        }
    }

    [DataSourceProperty]
    public BasicTooltipViewModel Hint
    {
        get { return _hint; }
        set
        {
            if (value != _hint)
            {
                _hint = value;
                OnPropertyChangedWithValue(value, "Hint");
            }
        }
    }

    [DataSourceProperty]
    public int Level
    {
        get { return _level; }
        set
        {
            if (value != _level)
            {
                _level = value;
                OnPropertyChangedWithValue(value, "Level");
            }
        }
    }

    [DataSourceProperty]
    public int PerkState
    {
        get { return _perkState; }
        set
        {
            if (value != _perkState)
            {
                _perkState = value;
                OnPropertyChangedWithValue(value, "PerkState");
            }
        }
    }

    [DataSourceProperty]
    public int AlternativeType
    {
        get { return _alternativeType; }
        set
        {
            if (value != _alternativeType)
            {
                _alternativeType = value;
                OnPropertyChangedWithValue(value, "AlternativeType");
            }
        }
    }

    [DataSourceProperty]
    public string LevelText
    {
        get { return _levelText; }
        set
        {
            if (value != _levelText)
            {
                _levelText = value;
                OnPropertyChangedWithValue(value, "LevelText");
            }
        }
    }

    [DataSourceProperty]
    public string BackgroundImage
    {
        get { return _backgroundImage; }
        set
        {
            if (value != _backgroundImage)
            {
                _backgroundImage = value;
                OnPropertyChangedWithValue(value, "BackgroundImage");
            }
        }
    }

    [DataSourceProperty]
    public string PerkId
    {
        get { return _perkId; }
        set
        {
            if (value != _perkId)
            {
                _perkId = value;
                OnPropertyChangedWithValue(value, "PerkId");
            }
        }
    }


    private void ExecuteShowPerkConcept()
    {
        if (_perkConceptObj != null)
        {
            Campaign.Current.EncyclopediaManager.GoToLink(_perkConceptObj.EncyclopediaLink);
        }
    }

    private void ExecuteStartSelection()
    {
        if (_onPerkSelectedChange != null && !_hasAlternativeAndSelected)
        {
            bool currentlySelected = _getIsPerkSelected(Perk);
            _onPerkSelectedChange(Perk, !currentlySelected);
            RefreshState();
        }
    }
}