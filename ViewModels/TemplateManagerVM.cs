using System;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.State;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerVM : ViewModel
{
    private TemplateManagerCharacter _templateManagerCharacter;
    private string _cancelLbl;
    private string _doneLbl;
    private TemplateManagerState _templateManagerState;
    TemplateManagerSkillGridVM _templateManagerSkillGridVM;

    public TemplateManagerVM(TemplateManagerState templateManagerState)
    {
        templateManagerState.EditTemplate = new TemplateManagerCharacter();
        templateManagerState.EditTemplate.CreateNewTemplate();
        _templateManagerCharacter = templateManagerState.EditTemplate;
        _templateManagerState = templateManagerState;
        _templateManagerSkillGridVM = new TemplateManagerSkillGridVM(_templateManagerCharacter);

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

    public void ExecuteCancel()
    {
        Close();
    }

    public void ExecuteDone()
    {
        Close();
    }

    private void Close()
    {
        GameStateManager.Current.PopState();
    }

    private void RefreshValues()
    {
        OnPropertyChanged(nameof(CancelLbl));
        OnPropertyChanged(nameof(DoneLbl));
    }
}