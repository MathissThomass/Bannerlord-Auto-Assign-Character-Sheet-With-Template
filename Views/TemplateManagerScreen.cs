using System.Collections.Generic;
using HeroPathAutoBuildAssignment.Utils;
using HeroPathAutoBuildAssignment.Behaviors;
using HeroPathAutoBuildAssignment.Data;
using HeroPathAutoBuildAssignment.State;
using HeroPathAutoBuildAssignment.ViewModels;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace HeroPathAutoBuildAssignment.Views;

[GameStateScreen(typeof(TemplateManagerState))]
public class TemplateManagerScreen : ScreenBase, IGameStateListener
{
    private TemplateManagerVM? _dataSource;
    private GauntletLayer? _layer;
    private TemplateManagerState _templateManagerState;
    private SpriteCategory _clanCategory;
    private SpriteCategory _navalCategory;
    private SpriteCategory _townManagementCategory;
    public TemplateStore TemplateStore { get; set; }
    

    public TemplateManagerScreen(TemplateManagerState state)
    {

        _templateManagerState = state;
    }

    protected override void OnFrameTick(float dt)
    {
        base.OnFrameTick(dt);

        //caused to exit even with an Inquiry open and the inquiry exit before this is executed so can't just close the inquiry with this code active
        /*if (Input.IsKeyReleased(InputKey.Escape))
        {
            _dataSource.ExecuteCancel();
        }

        if (Input.IsKeyReleased(InputKey.Enter) || Input.IsKeyReleased(InputKey.NumpadEnter))
        {
            _dataSource.ExecuteDone();
        }*/
    }

    void IGameStateListener.OnActivate()
    {
        SpriteData spriteData = UIResourceManager.SpriteData;
        TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
        ResourceDepot uIResourceDepot = UIResourceManager.ResourceDepot;
        
        _clanCategory = spriteData.SpriteCategories["ui_clan"];
        _navalCategory = spriteData.SpriteCategories["ui_naval_character_developer"];
        _townManagementCategory = spriteData.SpriteCategories["ui_town_management"];
        _clanCategory.Load(resourceContext, uIResourceDepot);
        _navalCategory.Load(resourceContext, uIResourceDepot);
        _townManagementCategory.Load(resourceContext, uIResourceDepot);
        
        _layer = new GauntletLayer("TemplateManagerLayer", 11, true);

        _dataSource = new TemplateManagerVM(_templateManagerState);
        _layer.LoadMovie("TemplateManagerScreen", _dataSource);
        _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
        ScreenManager.TrySetFocus(_layer);
        AddLayer(_layer);
    }

    void IGameStateListener.OnDeactivate()
    {
        HashSet<string> heroesAssignedAtOpen = _dataSource?.HeroesAssignedAtOpen;
        _layer.InputRestrictions.ResetInputRestrictions();
        RemoveLayer(_layer);
        _dataSource = null;
        TemplateStore.Instance.LoadFromDisk();
        CatchUpHeroesSavedDuringSession(heroesAssignedAtOpen);
    }

    private static void CatchUpHeroesSavedDuringSession(HashSet<string> heroesAssignedAtOpen)
    {
        var alreadyAssigned = heroesAssignedAtOpen ?? new HashSet<string>();
        var processedHeroIds = new HashSet<string>();
        foreach (var template in TemplateStore.Instance.TemplateList)
        {
            if (template?.HeroList == null)
            {
                continue;
            }

            foreach (var hero in template.HeroList)
            {
                if (hero == null || string.IsNullOrEmpty(hero.StringId) ||
                    !processedHeroIds.Add(hero.StringId) ||
                    alreadyAssigned.Contains(hero.StringId))
                {
                    continue;
                }

                AutoAssign.CatchUpAssignedHero(hero, template);
            }
        }
    }

    void IGameStateListener.OnInitialize()
    {
    }

    void IGameStateListener.OnFinalize()
    {
    }
}