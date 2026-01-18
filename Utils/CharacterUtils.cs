using System.Collections.Generic;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
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

    public static List<PerkObject> GetPerksForSkillInRange(SkillObject skill, int minLevel, int maxLevel)
    {
        var allPerks = PerkObject.All;

        var perkObjectList = allPerks
            .Where(perk => perk.Skill == skill && perk.RequiredSkillValue >= minLevel &&
                           perk.RequiredSkillValue <= maxLevel).OrderBy(p => p.RequiredSkillValue).ToList();

        return perkObjectList;
    }

    public static TemplateManagerCharacter? FindHeroAssignedTemplate(Hero hero,
        List<TemplateManagerCharacter> templateList)
    {
        foreach (var template in templateList)
        {
            if (template.HeroList.Contains(hero))
            {
                return template;
            }
        }

        return null;
    }
}