using System.Collections.Generic;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.Utils;

public class CharacterUtils
{
    public static IReadOnlyList<SkillObject> GetSkillsWithWarSails()
    {
        return new List<SkillObject>
        {
            DefaultSkills.OneHanded,
            DefaultSkills.Bow,
            DefaultSkills.Riding,
            DefaultSkills.Scouting,
            DefaultSkills.Charm,
            DefaultSkills.Steward,
            Game.Current.ObjectManager.GetObject<SkillObject>("Mariner"),
            DefaultSkills.TwoHanded,
            DefaultSkills.Crossbow,
            DefaultSkills.Athletics,
            DefaultSkills.Tactics,
            DefaultSkills.Leadership,
            DefaultSkills.Medicine,
            Game.Current.ObjectManager.GetObject<SkillObject>("Boatswain"),
            DefaultSkills.Polearm,
            DefaultSkills.Throwing,
            DefaultSkills.Crafting,
            DefaultSkills.Roguery,
            DefaultSkills.Trade,
            DefaultSkills.Engineering,
            Game.Current.ObjectManager.GetObject<SkillObject>("Shipmaster"),
        };
    }
}