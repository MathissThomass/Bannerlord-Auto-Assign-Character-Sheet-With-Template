using TaleWorlds.CampaignSystem;

namespace AutoAssignCharacterSheetWithTemplate.Utils;

public class HeroSelectionItem
{
    public Hero Hero { get; set; }
    public bool IsEnabled { get; set; } //Enable = can be clicked, Disable = is in another template
    public bool IsSelected { get; set; } // IsSelected = In the current template
}