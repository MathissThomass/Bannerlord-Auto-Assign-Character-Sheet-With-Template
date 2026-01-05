using System;
using System.IO;
using AutoAssignCharacterSheetWithTemplate._Module.GUI.Prefabs.TemplateScreens.State;
using AutoAssignCharacterSheetWithTemplate.ViewModels;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;
using Path = System.IO.Path;


namespace AutoAssignCharacterSheetWithTemplate.Views;

[GameStateScreen(typeof(TemplateManagerState))]
public class TemplateManagerScreen : ScreenBase, IGameStateListener
{
    private TemplateManagerVM? _dataSource;
    private GauntletLayer? _layer;
    private TemplateManagerState _templateManagerState;

    /*protected override void OnInitialize()
    {
        base.OnInitialize();
        LoadingWindow.EnableGlobalLoadingWindow();
    }*/

    public TemplateManagerScreen(TemplateManagerState state)
    {
        /*
        LoadingWindow.EnableGlobalLoadingWindow();
        */
        _templateManagerState = state;
    }

    protected override void OnFrameTick(float dt)
    {
        base.OnFrameTick(dt);
        if (_layer.Input.IsKeyPressed(InputKey.Escape))
        {
            _dataSource.ExecuteCancel();
        }

        if (_layer.Input.IsKeyPressed(InputKey.Enter))
        {
            _dataSource.ExecuteDone();
        }
    }

    /*

    public void OnExit()
    {
        Game.Current.GameStateManager.PopState(0);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if (_layer != null)
        {
            _layer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_layer);
        }
    }
    
    protected override void OnFinalize()
    {
        base.OnFinalize();
        if (LoadingWindow.IsLoadingWindowActive)
        {
            LoadingWindow.DisableGlobalLoadingWindow();
        }

        _dataSource = null;
        _layer = null;
    }*/

    void IGameStateListener.OnActivate()
    {
        _layer = new GauntletLayer("TemplateManagerLayer", 11, true);

        _dataSource = new TemplateManagerVM(_templateManagerState);
        _layer.LoadMovie("TemplateManagerScreen", _dataSource);
        _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
        ScreenManager.TrySetFocus(_layer);
        AddLayer(_layer);
        
    }

    void IGameStateListener.OnDeactivate()
    {
        _layer.InputRestrictions.ResetInputRestrictions();
        RemoveLayer(_layer);
        _dataSource = null;
    }

    void IGameStateListener.OnInitialize()
    {
    }

    void IGameStateListener.OnFinalize()
    {
    }
}