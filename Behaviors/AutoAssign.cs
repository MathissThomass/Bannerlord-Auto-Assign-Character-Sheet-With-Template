using System;
using System.Collections.Generic;
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
    private const double ImportantMultiplier = 1.25;
    private const double SpecialSkillNotImportantMultiplier = 0.6;
    private const double SpecialSkillImportantMultiplier = 1.4;
    private const double LearningRateCapMultiplier = 9.0;

    private static readonly HashSet<string> SpecialSkillIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { DefaultSkills.Steward.StringId, DefaultSkills.Medicine.StringId, DefaultSkills.Engineering.StringId };

    
    public static void AutoAssignFocusPoint(Hero hero, TemplateManagerCharacter template)
    {

        while (hero.HeroDeveloper.UnspentFocusPoints > 0)
        {
            var weightList = new Dictionary<SkillObject, double>();

            
            foreach (var templateSkill in template.SkillList)
            {
                var skill = templateSkill.Skill;
                if (!hero.HeroDeveloper.CanAddFocusToSkill(skill))
                {
                    weightList[skill] = 0;
                    continue;
                }

                var isImportant = templateSkill.IsSkillImportant;
                var learningRate = CharacterUtils.GetLearningRate(hero, skill);

                var baseWeight = 1.0 / learningRate;
                var mult = isImportant ? ImportantMultiplier : 1.0;
                if (SpecialSkillIds.Contains(skill.StringId))
                {
                    mult = isImportant ? SpecialSkillImportantMultiplier : SpecialSkillNotImportantMultiplier;
                }
                double weight = baseWeight;
                var learningRateAfterApplyFocus = CharacterUtils.GetLearningRate(hero, skill, 1);
                if ( learningRateAfterApplyFocus > LearningRateCapMultiplier)
                {
                    weight /= learningRateAfterApplyFocus;
                }
                else
                {
                    weight *= mult;
                }

                weightList[skill] = weight;
            }
            
            var pick = CharacterUtils.PickSkillByWeight(weightList);

            if (pick == null)
            {
                break;
            }
            
            hero.HeroDeveloper.AddFocus(pick, 1);
        }
    }
    
    public static void AutoAssignAttributPoint(Hero hero, TemplateManagerCharacter template)
    {
        while(hero.HeroDeveloper.UnspentAttributePoints > 0)
        {
            var weightList = new Dictionary<SkillObject, double>();

            
            foreach (var templateSkill in template.SkillList)
            {
                var skill = templateSkill.Skill;
                if (!hero.HeroDeveloper.CanAddFocusToSkill(skill))
                {
                    weightList[skill] = 0;
                    continue;
                }

                var isImportant = templateSkill.IsSkillImportant;
                var learningRate = CharacterUtils.GetLearningRate(hero, skill);

                var baseWeight = 1.0 / learningRate;
                var mult = isImportant ? ImportantMultiplier : 1.0;
                if (SpecialSkillIds.Contains(skill.StringId))
                {
                    mult = isImportant ? SpecialSkillImportantMultiplier : SpecialSkillNotImportantMultiplier;
                }
                double weight = baseWeight * mult;

                weightList[skill] = weight;
            }
            
            var pick = CharacterUtils.PickAttributByWeight(weightList);

            if (pick == null)
            {
                break;
            }
            
            hero.HeroDeveloper.AddAttribute(pick, 1);
        }
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