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
    private string _cancelLbl;
    private string _doneLbl;
    private List<TemplateManagerCharacter> _templateManagerCharacterList;
    TemplateManagerSkillGridVM _templateManagerSkillGridVM;
    private TemplateListVM _templateListVM;

    public TemplateManagerVM(TemplateManagerState templateManagerState)
    {
        templateManagerState.EditTemplate = new TemplateManagerCharacter();
        templateManagerState.EditTemplate.CreateNewTemplate();
        _templateManagerCharacterList = new List<TemplateManagerCharacter>();
        _templateManagerCharacterList = TemplateSaveManager.LoadSavedTemplatesList();
        // Ensure we have at least one template, otherwise use the edit template
        if (_templateManagerCharacterList.Count > 0)
        {
            _currentTemplate = _templateManagerCharacterList[0]; //TODO if hero from characterdev have a template select this one 
        }
        else
        {
            _currentTemplate = templateManagerState.EditTemplate;
        }
        _templateManagerSkillGridVM = new TemplateManagerSkillGridVM(_currentTemplate);
        _templateListVM = new TemplateListVM(_templateManagerCharacterList);

        _cancelLbl = "Annuler";
        _doneLbl = "Valider";
        RefreshValues();
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
            return _templateManagerSkillGridVM;
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

    public void ExecuteCancel()
    {
        Close();
    }

    public void ExecuteDone()
    {
        
    }

    private void Close()
    {
        GameStateManager.Current.PopState();
    }

    private void RefreshValues()
    {
        OnPropertyChanged(nameof(CancelLbl));
        OnPropertyChanged(nameof(DoneLbl));
        OnPropertyChanged(nameof(TemplateListVM));
        OnPropertyChanged(nameof(TemplateSkillGridVM));
    }
}