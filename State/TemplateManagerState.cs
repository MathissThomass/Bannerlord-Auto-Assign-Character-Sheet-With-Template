using AutoAssignCharacterSheetWithTemplate.Models;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.State;

public class TemplateManagerState : GameState
{
    public TemplateManagerCharacter EditTemplate { get; set; }
        
    public TemplateManagerState()
    {
    }


    public override bool IsMenuState
    {
        get { return true; }
    }
}