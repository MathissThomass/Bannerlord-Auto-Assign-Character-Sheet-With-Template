using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.State;

public class TemplateManagerState : GameState
{
    public TemplateManagerState()
    {
    }


    public override bool IsMenuState
    {
        get { return true; }
    }
}