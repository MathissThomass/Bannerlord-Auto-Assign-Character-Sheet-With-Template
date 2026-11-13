using Bannerlord.UIExtenderEx;
using HarmonyLib;
using HeroPathAutoBuildAssignment.Behaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;


namespace HeroPathAutoBuildAssignment
{
    public class Main : MBSubModuleBase
    {
        private Harmony? _harmony;
        private UIExtender? _uiExtender;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            _harmony = new Harmony("HeroPathAutoBuildAssignment");
            _harmony.PatchAll();
            
            _uiExtender = new UIExtender("HeroPathAutoBuildAssignment");
            _uiExtender.Register(typeof(Main).Assembly);
            _uiExtender.Enable();
        }

        protected override void OnSubModuleUnloaded()
        {
            _uiExtender?.Disable();
            
            _harmony?.UnpatchAll("HeroPathAutoBuildAssignment");
            _harmony = null;
            
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }
        
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            if (game.GameType is Campaign)
            {
                var starter = (CampaignGameStarter)gameStarterObject;
                AddBehaviors(starter);
            }
        }

        private static void AddBehaviors(CampaignGameStarter starter)
        {
            starter.AddBehavior(new AutoAssignBehavior());
        }
    }
}