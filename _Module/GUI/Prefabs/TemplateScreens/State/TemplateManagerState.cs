using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate._Module.GUI.Prefabs.TemplateScreens.State;

public class TemplateManagerState : GameState
{

    public TemplateManagerState()
    {
    }
    

    public override bool IsMenuState
    {
        get
        {
            return true;
        }
    }
}