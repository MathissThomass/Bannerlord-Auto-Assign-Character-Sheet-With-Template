using System;
using System.Collections.Generic;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateHeroSelectionPopupVM : ViewModel
{
    private MBBindingList<HeroSelectionItemVM> _heroItemList;

    private TemplateManagerCharacter _currentTemplate;

    public TemplateHeroSelectionPopupVM()
    {
        _heroItemList = new MBBindingList<HeroSelectionItemVM>();
    }

    public void OpenForTemplate(TemplateManagerCharacter template, List<TemplateManagerCharacter> templateList)
    {
        _currentTemplate = template;
        RefreshItemList(templateList);
    }

    public void Close()
    {
        _currentTemplate = null;
        HeroItemList.Clear();
        OnPropertyChanged("HeroItemList");
    }

    private void RefreshItemList(List<TemplateManagerCharacter> templateList)
    {
        HeroItemList.Clear();
        var list = TemplateHeroUtils.GetHeroSelectionEntries(templateList, _currentTemplate);
        foreach (var heroSelectionItem in list)
        {
            HeroItemList.Add(new HeroSelectionItemVM(heroSelectionItem.Hero, heroSelectionItem.IsEnabled, heroSelectionItem.IsSelected, OnHeroSelected));
        }
        OnPropertyChanged("HeroItemList");
    }

    private void OnHeroSelected(bool isSelected, Hero hero)
    {
        if (isSelected)
        {
            _currentTemplate.HeroList.Add(hero);
        }
        else
        {
            _currentTemplate.HeroList.Remove(hero);
        }
    }

    [DataSourceProperty]
    public MBBindingList<HeroSelectionItemVM> HeroItemList
    {
        get => _heroItemList;
        set
        {
            if (value != _heroItemList)
            {
                _heroItemList = value;
                OnPropertyChanged("HeroItemList");
            }
        }
    }
}