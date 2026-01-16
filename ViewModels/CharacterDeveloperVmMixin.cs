using AutoAssignCharacterSheetWithTemplate.State;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

[ViewModelMixin("RefreshValues")]
internal class CharacterDeveloperVmMixin : BaseViewModelMixin<CharacterDeveloperVM>
{
    private readonly CharacterDeveloperVM _vm;

    public CharacterDeveloperVmMixin(CharacterDeveloperVM vm) : base(vm)
    {
        _vm = vm;
    }

    public override void OnRefresh()
    {
        base.OnRefresh();
    }

    [DataSourceMethod]
    public void ExecuteOpenTemplateManager() => OpenTemplate();

    private void OpenTemplate()
    {
        var gameState = GameStateManager.Current.CreateState<TemplateManagerState>();
        GameStateManager.Current.PushState(gameState, 0);
    }
}