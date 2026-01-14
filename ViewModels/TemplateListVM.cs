using System;
using System.Collections.Generic;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateListVM : ViewModel
{
    List<TemplateManagerCharacter> _templateManagerCharacterList;
    MBBindingList<TemplateListItemVM> _listItemVm;
    TemplateListItemVM _currentTemplateListItemVM;
    Action<TemplateManagerCharacter, int> _onTemplateSelectedChanged;
    private Action<TemplateManagerCharacter> _onCreateNewTemplate;

    public TemplateListVM(List<TemplateManagerCharacter> templateManagerCharacterList, Action<TemplateManagerCharacter, int> onTemplateSelectedChanged, Action<TemplateManagerCharacter> onCreateNewTemplate)
    {
        _templateManagerCharacterList = templateManagerCharacterList;
        _listItemVm = new MBBindingList<TemplateListItemVM>();
        _onTemplateSelectedChanged = onTemplateSelectedChanged;
        _onCreateNewTemplate = onCreateNewTemplate;
        RefreshTemplateList(0); // TODO change to the one selected if hero had a temlplate
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
            ListItemVM[inspectedIndex].IsInspected = true;
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
        _onCreateNewTemplate(newTemplate);
    }

    private void ExecuteDuplicateCurrentTemplate()
    {
        var original = _currentTemplateListItemVM._templateManagerCharacter; var json = JsonConvert.SerializeObject(original); 
        var newTemplate = JsonConvert.DeserializeObject<TemplateManagerCharacter>(json);
        newTemplate.Name = "Duplicate " + newTemplate.Name;
        newTemplate.CreateNewTemplate();
        _templateManagerCharacterList.Add(newTemplate);
        var index = _templateManagerCharacterList.Count - 1;
        RefreshTemplateList(index);
        _onCreateNewTemplate(newTemplate);
    }

    private void ExecuteOpenHeroListToAssign()
    {
        //TODO
    }

    public void OnTemplateSelectedChanged(TemplateListItemVM templateListItemVm)
    {
        if (templateListItemVm != _currentTemplateListItemVM)
        {
            if (_currentTemplateListItemVM != null)
            {
                _currentTemplateListItemVM.IsInspected = false;
            }

            CurrentTemplateItemVM = templateListItemVm;
            int index = _listItemVm.IndexOf(templateListItemVm);
            _onTemplateSelectedChanged(CurrentTemplateItemVM._templateManagerCharacter, index);
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