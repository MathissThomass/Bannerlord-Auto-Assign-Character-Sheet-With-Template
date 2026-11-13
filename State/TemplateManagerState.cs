using HeroPathAutoBuildAssignment.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace HeroPathAutoBuildAssignment.State;

public class TemplateManagerState : GameState
{
    public TemplateManagerCharacter EditTemplate { get; set; }

    public Hero? SelectedHero { get; set; }
        
    public TemplateManagerState()
    {
    }


    public override bool IsMenuState
    {
        get { return true; }
    }
}