using System.Collections.Generic;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

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
        RefreshTemplateList(0);
    }

    private void RefreshTemplateList(int inspectedIndex)
    {
        ListItemVM.Clear();
        foreach (var templateCharacter in _templateManagerCharacterList)
        {
            ListItemVM.Add(new TemplateListItemVM(templateCharacter, OnTemplateSelectedChanged));
        }

        if (ListItemVM.Count > 0)
        {
            ListItemVM[inspectedIndex].IsInspected = true; // TODO change to the one selected if hero had a temlplate
            _currentTemplateListItemVM = ListItemVM[inspectedIndex];
        }

        OnPropertyChanged("ListItemVM");
    }

    private void ExecuteDeleteCurrent()
    {
        var inquiry = new InquiryData(
            titleText: "Delete current template",
            text: "Are you sure you want to delete the current template?",
            isAffirmativeOptionShown: true,
            isNegativeOptionShown: true,
            affirmativeText: "Delete",
            negativeText: "Cancel",
            affirmativeAction: OnDeleteCurrentTemplate,
            negativeAction: InformationManager.HideInquiry
        );
        InformationManager.ShowInquiry(inquiry);
    }

    private void ExecuteCreateNew()
    {
        var newTemplate = new TemplateManagerCharacter
        {
            Name = "New created template"
        };
        newTemplate.CreateNewTemplate();
        _templateManagerCharacterList.Add(newTemplate);
        var index = _templateManagerCharacterList.Count - 1;
        RefreshTemplateList(index);
    }

    private void ExecuteDuplicateCurrentTemplate()
    
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

    private void OnDeleteCurrentTemplate()
    {
        TemplateSaveManager.DeleteTemplate(_currentTemplateListItemVM.TemplateName);
        _templateManagerCharacterList = TemplateSaveManager.LoadSavedTemplatesList();
        RefreshTemplateList(0);
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