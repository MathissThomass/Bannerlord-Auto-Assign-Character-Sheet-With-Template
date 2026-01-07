using System.Collections.Generic;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.Utils;

public class CharacterUtils
{
    
    /// <summary>
    /// Retourne la liste des compétences de base du jeu (vanilla) à transférer.
    /// </summary>
    public static IReadOnlyList<SkillObject> GetVanillaSkills()
    {
        return new List<SkillObject>
        {
            DefaultSkills.OneHanded,
            DefaultSkills.TwoHanded,
            DefaultSkills.Polearm,
            DefaultSkills.Bow,
            DefaultSkills.Crossbow,
            DefaultSkills.Throwing,
            DefaultSkills.Riding,
            DefaultSkills.Athletics,
            DefaultSkills.Crafting,
            DefaultSkills.Scouting,
            DefaultSkills.Tactics,
            DefaultSkills.Roguery,
            DefaultSkills.Charm,
            DefaultSkills.Leadership,
            DefaultSkills.Trade,
            DefaultSkills.Steward,
            DefaultSkills.Medicine,
            DefaultSkills.Engineering,
        };
    }
    
    /// <summary>
    /// Retourne la liste des compétences du mod WarSails (si disponibles) à transférer.
    /// </summary>
    public static IReadOnlyList<SkillObject> GetWarSailsSkills()
    {
        string[] warSailSkillIds =
        {
            "Mariner",
            "Boatswain",
            "Shipmaster"
        };

        var warSailsSkills = new List<SkillObject>(warSailSkillIds.Length);

        foreach (var id in warSailSkillIds)
        {
            var skill = Game.Current.ObjectManager.GetObject<SkillObject>(id);
            if (skill != null)
            {
                warSailsSkills.Add(skill);
            }
        }

        return warSailsSkills;
    }

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