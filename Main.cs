using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.MountAndBlade;


namespace AutoAssignCharacterSheetWithTemplate
{
    public class Main : MBSubModuleBase
    {
        private Harmony? _harmony;
        private UIExtender? _uiExtender;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            _harmony = new Harmony("AutoAssignCharacterSheetWithTemplate");
            _harmony.PatchAll();
            
            _uiExtender = new UIExtender("AutoAssignCharacterSheetWithTemplate");
            _uiExtender.Register(typeof(Main).Assembly);
            _uiExtender.Enable();
        }

        protected override void OnSubModuleUnloaded()
        {
            _uiExtender?.Disable();
            
            _harmony?.UnpatchAll("AutoAssignCharacterSheetWithTemplate");
            _harmony = null;
            
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }
    }
}