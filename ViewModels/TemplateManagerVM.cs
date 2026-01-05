using System;
using AutoAssignCharacterSheetWithTemplate._Module.GUI.Prefabs.TemplateScreens.State;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

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
        try
        {
            GameStateManager.Current.PopState();
        }
        catch (Exception ex)
        {
            InformationManager.DisplayMessage(new InformationMessage("Close error: " + ex.Message));
        }
    }

    private void RefreshValues()
    {
        OnPropertyChanged(nameof(CancelLbl));
        OnPropertyChanged(nameof(DoneLbl));
    }
}