using System;
using System.Collections.Generic;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateListVM : ViewModel
{
    public List<TemplateManagerCharacter> _templateManagerCharacterList;
    MBBindingList<TemplateListItemVM> _listItemVm;
    TemplateListItemVM _currentTemplateListItemVM;
    Action<TemplateManagerCharacter, int> _onTemplateSelectedChanged;
    private Action<TemplateManagerCharacter> _onCreateNewTemplate;
    private TemplateHeroSelectionPopupVM _heroSelectionPopupVm;
    private bool _isHeroListOpen;

    public TemplateListVM(List<TemplateManagerCharacter> templateManagerCharacterList,
        Action<TemplateManagerCharacter, int> onTemplateSelectedChanged,
        Action<TemplateManagerCharacter> onCreateNewTemplate)
    {
        _templateManagerCharacterList = templateManagerCharacterList;
        _listItemVm = new MBBindingList<TemplateListItemVM>();
        _onTemplateSelectedChanged = onTemplateSelectedChanged;
        _onCreateNewTemplate = onCreateNewTemplate;
        _heroSelectionPopupVm = new TemplateHeroSelectionPopupVM();
        _isHeroListOpen = false;
        RefreshTemplateList(0); // TODO change to the one selected if hero had a temlplate
    }

    public void RefreshTemplateList(int inspectedIndex)
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

        if (IsHeroListOpen)
        {
            _heroSelectionPopupVm.OpenForTemplate(_currentTemplateListItemVM._templateManagerCharacter, _templateManagerCharacterList);
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
            Name = GenerateUniqueName("New created template")
        };
        newTemplate.CreateNewTemplate();
        _templateManagerCharacterList.Add(newTemplate);
        var index = _templateManagerCharacterList.Count - 1;
        RefreshTemplateList(index);
        _onCreateNewTemplate(newTemplate);
    }

    private void ExecuteDuplicateCurrentTemplate()
    {
        var original = _currentTemplateListItemVM._templateManagerCharacter;
        var json = JsonConvert.SerializeObject(original);
        var newTemplate = JsonConvert.DeserializeObject<TemplateManagerCharacter>(json);
        newTemplate.Name = GenerateUniqueName("Duplicate " + newTemplate.Name);
        newTemplate.CreateNewTemplate();
        _templateManagerCharacterList.Add(newTemplate);
        var index = _templateManagerCharacterList.Count - 1;
        RefreshTemplateList(index);
        _onCreateNewTemplate(newTemplate);
    }

    private void ExecuteRenameCurrentTemplate()
    {
        var textInquiry = new TextInquiryData(
            titleText: "Enter the template name",
            text: "New template name :",
            isAffirmativeOptionShown: true,
            isNegativeOptionShown: true,
            affirmativeText: GameTexts.FindText("str_done", null).ToString(),
            negativeText: GameTexts.FindText("str_cancel", null).ToString(),
            affirmativeAction: OnEnterNameAfter,
            negativeAction: InformationManager.HideInquiry,
            textCondition: TemplateSaveManager.CheckIfNameExists,
            defaultInputText: _currentTemplateListItemVM.TemplateName
        );
        InformationManager.ShowTextInquiry(textInquiry);
    }

    private void ExecuteOpenHeroListToAssign()
    {
        _isHeroListOpen = !_isHeroListOpen;
        OnPropertyChanged("IsHeroListOpen");
        OnPropertyChanged("HeroListButtonBrushStringId");
        if (_isHeroListOpen)
        {
            var currentTemplate = _currentTemplateListItemVM._templateManagerCharacter;
            _heroSelectionPopupVm.OpenForTemplate(currentTemplate, _templateManagerCharacterList);
        }
        else
        {
            _heroSelectionPopupVm.Close();
        }
    }

    private void OnEnterNameAfter(string newName)
    {
        if (!_currentTemplateListItemVM._templateManagerCharacter.GetIsFromNewCreatedTemplate())
        {
            TemplateSaveManager.RenameSaveFile(newName, _currentTemplateListItemVM.TemplateName);
        }

        var index = _listItemVm.FindIndex(obj => obj.Equals(_currentTemplateListItemVM));
        _currentTemplateListItemVM.TemplateName = newName;
        _templateManagerCharacterList[index].Name = newName;
        RefreshTemplateList(index);
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
            _heroSelectionPopupVm.OpenForTemplate(templateListItemVm._templateManagerCharacter, _templateManagerCharacterList);
        }
    }

    private void OnDeleteCurrentTemplate()
    {
        if (!_currentTemplateListItemVM._templateManagerCharacter.GetIsFromNewCreatedTemplate())
        {
            TemplateSaveManager.DeleteTemplate(_currentTemplateListItemVM.TemplateName);
        }

        var result = _templateManagerCharacterList.Find(obj => obj.Name == _currentTemplateListItemVM.TemplateName);
        _templateManagerCharacterList.Remove(result);
        RefreshTemplateList(0);
    }

    private string GenerateUniqueName(string baseName)
    {
        var existingNames = _templateManagerCharacterList.Select(t => t.Name).ToList();

        if (!existingNames.Contains(baseName))
        {
            return baseName;
        }

        int counter = 2;
        string candidate;

        do
        {
            candidate = $"{baseName} {counter}";
            counter++;
        } while (existingNames.Contains(candidate));
        return candidate;
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
    
    [DataSourceProperty]
    public string HeroListButtonBrushStringId
    {
        get { return _isHeroListOpen ? "ButtonBrush1" : "ButtonBrush2"; }
    }

    [DataSourceProperty]
    public bool IsHeroListOpen
    {
        get { return _isHeroListOpen; }
        set
        {
            if (value != _isHeroListOpen)
            {
                _isHeroListOpen = value;
                OnPropertyChangedWithValue(value, "IsHeroListOpen");
            }
        }
    }
    
    [DataSourceProperty]
    public TemplateHeroSelectionPopupVM HeroSelectionPopupVm
    {
        get { return _heroSelectionPopupVm; }
        set
        {
            if (value != _heroSelectionPopupVm)
            {
                _heroSelectionPopupVm = value;
                OnPropertyChangedWithValue(value, "HeroSelectionPopupVm");
            }
        }
    }
}