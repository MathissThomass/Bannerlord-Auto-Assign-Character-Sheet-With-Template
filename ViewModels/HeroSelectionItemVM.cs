using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class HeroSelectionItemVM : ViewModel
{
    private bool _isEnabled;
    private bool _isSelected;
    private string _nameText;
    private CharacterImageIdentifierVM _visual;
    public readonly Hero Hero;
    private Action<bool, Hero> _onHeroSelected;

    public HeroSelectionItemVM(Hero hero, bool isEnabled, bool isSelected, Action<bool, Hero> onHeroSelected)
    {
        Hero = hero;
        _isEnabled = isEnabled;
        _isSelected = isSelected;
        _nameText = hero?.Name?.ToString() ?? "Unknown";
        _visual = new CharacterImageIdentifierVM(CampaignUIHelper.GetCharacterCode(Hero.CharacterObject, true));
        _onHeroSelected = onHeroSelected;
    }

    private void ExecuteToggleHero()
    {
        if (!IsEnabled)
        {
            return;
        } 

        IsSelected = !IsSelected;
        _onHeroSelected(IsSelected, Hero);
    }
    

    [DataSourceProperty]
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value != _isEnabled)
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value != _isSelected)
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }

    [DataSourceProperty]
    public string NameText
    {
        get => _nameText;
        set
        {
            if (value != _nameText)
            {
                _nameText = value;
                OnPropertyChanged("NameText");
            }
        }
    }

    [DataSourceProperty]
    public CharacterImageIdentifierVM Visual
    {
        get => _visual;
        set
        {
            if (value != _visual)
            {
                _visual = value;
                OnPropertyChanged("Visual");
            }
        }
    }
}