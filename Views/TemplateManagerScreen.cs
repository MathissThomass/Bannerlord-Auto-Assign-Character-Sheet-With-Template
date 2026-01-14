using AutoAssignCharacterSheetWithTemplate.State;
using AutoAssignCharacterSheetWithTemplate.ViewModels;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace AutoAssignCharacterSheetWithTemplate.Views;

[GameStateScreen(typeof(TemplateManagerState))]
public class TemplateManagerScreen : ScreenBase, IGameStateListener
{
    private TemplateManagerVM? _dataSource;
    private GauntletLayer? _layer;
    private TemplateManagerState _templateManagerState;
    private SpriteCategory _clanCategory;
    private SpriteCategory _navalCategory;

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
        if (Input.IsKeyReleased(InputKey.Escape))
        {
            _dataSource.ExecuteCancel();
        }

        if (TaleWorlds.InputSystem.Input.IsKeyReleased(InputKey.Enter))
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
        SpriteData spriteData = UIResourceManager.SpriteData;
        TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
        ResourceDepot uIResourceDepot = UIResourceManager.ResourceDepot;
        
        _clanCategory = spriteData.SpriteCategories["ui_clan"];
        _navalCategory = spriteData.SpriteCategories["ui_naval_character_developer"];
        _clanCategory.Load(resourceContext, uIResourceDepot);
        _navalCategory.Load(resourceContext, uIResourceDepot);
        
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