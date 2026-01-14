using System.Collections.Generic;
using AutoAssignCharacterSheetWithTemplate.Models;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateListVM : ViewModel
{
    List<TemplateManagerCharacter> _templateManagerCharacterList;
    MBBindingList<TemplateListItemVM> _listItemVm;
    TemplateListItemVM _currentTemplateListItemVM;

    public TemplateListVM(List<TemplateManagerCharacter> templateManagerCharacterList)
    {
        _templateManagerCharacterList = templateManagerCharacterList;
        _listItemVm = new MBBindingList<TemplateListItemVM>();
        RefreshTemplateList();
    }

    private void RefreshTemplateList()
    {
        ListItemVM.Clear();
        foreach (var templateCharacter in _templateManagerCharacterList)
        {
            ListItemVM.Add(new TemplateListItemVM(templateCharacter, OnTemplateSelectedChanged));
        }

        if (ListItemVM.Count > 0)
        {
            ListItemVM[0].IsInspected = true; // TODO change to the one selected if hero had a temlplate
            _currentTemplateListItemVM = ListItemVM[0];
        }

        OnPropertyChanged("ListItemVM");
    }

    private void ExecuteDeleteCurrent()
    {
        //TODO
    }

    private void ExecuteCreateNew()
    {
        //TODO
    }

    private void ExecuteOpenHeroListToAssign()
    {
        //TODO
    }

    public void OnTemplateSelectedChanged(TemplateListItemVM templateListVM)
    {
        if (templateListVM != _currentTemplateListItemVM)
        {
            if (_currentTemplateListItemVM != null)
            {
                _currentTemplateListItemVM.IsInspected = false;
            }

            CurrentTemplateItemVM = templateListVM;
        }
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
}