using System.Collections.Generic;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.State;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerVM : ViewModel
{
    private TemplateManagerCharacter _currentTemplate;
    private int _currentTemplateIndex;
    private string _cancelLbl;
    private string _doneLbl;
    private List<TemplateManagerCharacter> _templateManagerCharacterList;
    TemplateManagerSkillGridVM _currentSkillGridVM;
    private MBBindingList<TemplateManagerSkillGridVM> SkillGridListVm;
    private TemplateListVM _templateListVM;

    public TemplateManagerVM(TemplateManagerState templateManagerState)
    {
        templateManagerState.EditTemplate = new TemplateManagerCharacter();
        templateManagerState.EditTemplate.CreateNewTemplate();
        _templateManagerCharacterList = new List<TemplateManagerCharacter>();
        _templateManagerCharacterList = TemplateSaveManager.LoadSavedTemplatesList();
        if (_templateManagerCharacterList.Count > 0)
        {
            _currentTemplate = _templateManagerCharacterList[0]; //TODO if hero from characterdev have a template select this one
        }
        else
        {
            var newTemplate = new TemplateManagerCharacter
            {
                Name = "New created template"
            };
            newTemplate.CreateNewTemplate();
            _templateManagerCharacterList.Add(newTemplate);
            _currentTemplate = _templateManagerCharacterList[0];
        }

        SkillGridListVm = new MBBindingList<TemplateManagerSkillGridVM>();
        
        foreach (var templateManagerCharacter in _templateManagerCharacterList)
        {
            SkillGridListVm.Add(new TemplateManagerSkillGridVM(templateManagerCharacter, OnSaveTemplate));
        }
        
        _currentSkillGridVM = SkillGridListVm[0]; //TODO a changer
        _templateListVM = new TemplateListVM(_templateManagerCharacterList, OnTemplateCharacterSelectedChange, OnCreateNewTemplate);

        _cancelLbl = "Annuler";
        _doneLbl = "Valider";
        RefreshValues();
    }

    public void ExecuteCancel()
    {
        //TODO
        Close();
    }

    public void ExecuteDone()
    {
        //TODO
    }

    private void Close()
    {
        GameStateManager.Current.PopState();
    }

    private void OnTemplateCharacterSelectedChange(TemplateManagerCharacter templateManagerCharacter, int index)
    {
        _currentTemplate = templateManagerCharacter;
        _currentSkillGridVM = SkillGridListVm[index];
        _currentTemplateIndex = index;
        OnPropertyChanged("TemplateSkillGridVM");
    }

    private void OnCreateNewTemplate(TemplateManagerCharacter newTemplate)
    {
        newTemplate.SetIsFromNewCreatedTemplate(true);
        SkillGridListVm.Add(new TemplateManagerSkillGridVM(newTemplate, OnSaveTemplate));
        _currentTemplate = newTemplate;
        _currentSkillGridVM = new TemplateManagerSkillGridVM(_currentTemplate, OnSaveTemplate);
        _currentTemplateIndex = _templateListVM.ListItemVM.Count - 1;
        OnPropertyChanged("TemplateSkillGridVM");
    }

    private void OnSaveTemplate(TemplateManagerCharacter templateCharacter)
    {
        templateCharacter.SetIsFromNewCreatedTemplate(false);
        _currentTemplate = templateCharacter;
        _templateListVM._templateManagerCharacterList[_currentTemplateIndex] = templateCharacter;
        _templateListVM.RefreshTemplateList(_currentTemplateIndex);
    }

    private void RefreshValues()
    {
        OnPropertyChanged(nameof(CancelLbl));
        OnPropertyChanged(nameof(DoneLbl));
        OnPropertyChanged(nameof(TemplateListVM));
        OnPropertyChanged(nameof(TemplateSkillGridVM));
    }
    
    [DataSourceProperty]
    public string CancelLbl
    {
        get => _cancelLbl;
        set
        {
            if (_cancelLbl != value)
            {
                _cancelLbl = value;
                OnPropertyChanged(nameof(CancelLbl));
            }
        }
    }

    [DataSourceProperty]
    public string DoneLbl
    {
        get => _doneLbl;
        set
        {
            if (_doneLbl != value)
            {
                _doneLbl = value;
                OnPropertyChanged(nameof(DoneLbl));
            }
        }
    }

    [DataSourceProperty]
    public TemplateManagerSkillGridVM TemplateSkillGridVM
    {
        get
        {
            return _currentSkillGridVM;
        }
        set
        {
            if (_currentSkillGridVM != value)
            {
                _currentSkillGridVM = value;
                OnPropertyChangedWithValue(value, "TemplateSkillGridVM");
            }
        }
    }

    [DataSourceProperty]
    public TemplateListVM TemplateListVM
    {
        get
        {
            return _templateListVM;
        }
    }
}