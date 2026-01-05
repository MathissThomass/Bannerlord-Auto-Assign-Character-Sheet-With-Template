using System;
using AutoAssignCharacterSheetWithTemplate.State;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateManagerVM : ViewModel
{
    private string _cancelLbl;
    private string _doneLbl;
    private TemplateManagerState _templateManagerState;

    public TemplateManagerVM(TemplateManagerState templateManagerState)
    {
        _templateManagerState = templateManagerState;

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