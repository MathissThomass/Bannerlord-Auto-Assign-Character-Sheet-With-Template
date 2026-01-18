using System;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Models;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.Behaviors;

public class AutoAssign
{
    private static readonly Random _rng = new Random();


    public static void AutoAssignAttributPoint(Hero hero, TemplateManagerCharacter template)
    {
    }

    public static void AutoAssignFocusPoint(Hero hero, TemplateManagerCharacter template)
    {
    }

    public static void AutoAssignPerkPoint(Hero hero, TemplateManagerCharacter template, SkillObject skill,
        int initialLvl)
    {
        var unlockedPerks = CharacterUtils.GetPerksForSkillInRange(skill, initialLvl, hero.GetSkillValue(skill));
        var templatePerkList = template.PerkList.FindAll(perk => perk.Enable).Select(perk => perk.StringId).ToList();
        var unlockedPerkIdInTemplate =
            unlockedPerks.Select(perkObject => perkObject.StringId).Intersect(templatePerkList);
        var perkInTemplateToAdd = PerkObject.All
            .Where(perkObject => unlockedPerkIdInTemplate.Contains(perkObject.StringId))
            .ToList();

        foreach (var perk in from perk in perkInTemplateToAdd
                 let perkValue = hero.GetPerkValue(perk)
                 let alternativePerkValue = hero.GetPerkValue(perk.AlternativePerk)
                 where !perkValue && !alternativePerkValue
                 select perk)
        {
            hero.HeroDeveloper.AddPerk(perk);
        }

        var perkToRandomize = unlockedPerks.Except(perkInTemplateToAdd);
        foreach (var perk in perkToRandomize)
        {
            if (!hero.GetPerkValue(perk) && perk.AlternativePerk == null)
            {
                hero.HeroDeveloper.AddPerk(perk);
                continue;
            }

            if (hero.GetPerkValue(perk) || perk.AlternativePerk == null ||
                hero.GetPerkValue(perk.AlternativePerk)) continue;
            var rand = _rng.Next(2);

            hero.HeroDeveloper.AddPerk(rand == 0 ? perk : perk.AlternativePerk);
        }
    }
}