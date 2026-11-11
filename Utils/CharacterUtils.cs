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

    public static float GetLearningRate(Hero hero, SkillObject skill, int additionalFocus = 0)
    {
        var currentFocusLevel = hero.HeroDeveloper.GetFocus(skill);
        var characterAttributes = hero.CharacterAttributes;

        var skillLvl = hero.GetSkillValue(skill);
        var learningRate = Campaign.Current.Models.CharacterDevelopmentModel
            .CalculateLearningRate(characterAttributes, currentFocusLevel + additionalFocus, skillLvl, skill, false)
            .ResultNumber;

        return learningRate;
    }

    public static SkillObject? PickSkillByWeight(Dictionary<SkillObject, double> weights)
    {
        SkillObject? bestSkill = null;
        var bestWeight = double.NegativeInfinity;

        foreach (var kv in weights)
        {
            var skill = kv.Key;
            var weight = kv.Value;
            if (!(weight > bestWeight)) continue;
            bestWeight = weight;
            bestSkill = skill;
        }

        return bestWeight <= 0 ? null : bestSkill;
    }

    public static CharacterAttribute? PickAttributByWeight(Dictionary<SkillObject, double> weights)
    {
        var sumByAttribute = new Dictionary<CharacterAttribute, double>();

        foreach (var kv in weights)
        {
            var skill = kv.Key;
            var weight = kv.Value;

            var attributes = GetAttributesFromSkill(skill).ToList();
            if (attributes.Count == 0)
                continue;

            var distributedWeight = weight / attributes.Count;

            foreach (var attr in attributes)
            {
                if (sumByAttribute.ContainsKey(attr))
                    sumByAttribute[attr] += distributedWeight;
                else
                    sumByAttribute[attr] = distributedWeight;
            }
        }

        if (sumByAttribute.Count == 0)
            return null;

        return sumByAttribute
            .OrderByDescending(kv => kv.Value)
            .First().Key;
    }

    public static CharacterAttribute? GetAttributeFromSkill(SkillObject skill)
    {
        return skill != null && SkillToAttribute.TryGetValue(skill, out var attribute)
            ? attribute
            : null;
    }

    private static readonly Dictionary<SkillObject, CharacterAttribute> SkillToAttribute =
        new Dictionary<SkillObject, CharacterAttribute>
        {
            { DefaultSkills.OneHanded, DefaultCharacterAttributes.Vigor },
            { DefaultSkills.TwoHanded, DefaultCharacterAttributes.Vigor },
            { DefaultSkills.Polearm, DefaultCharacterAttributes.Vigor },

            { DefaultSkills.Bow, DefaultCharacterAttributes.Control },
            { DefaultSkills.Crossbow, DefaultCharacterAttributes.Control },
            { DefaultSkills.Throwing, DefaultCharacterAttributes.Control },

            { DefaultSkills.Riding, DefaultCharacterAttributes.Endurance },
            { DefaultSkills.Athletics, DefaultCharacterAttributes.Endurance },
            { DefaultSkills.Crafting, DefaultCharacterAttributes.Endurance },

            { DefaultSkills.Scouting, DefaultCharacterAttributes.Cunning },
            { DefaultSkills.Tactics, DefaultCharacterAttributes.Cunning },
            { DefaultSkills.Roguery, DefaultCharacterAttributes.Cunning },

            { DefaultSkills.Charm, DefaultCharacterAttributes.Social },
            { DefaultSkills.Leadership, DefaultCharacterAttributes.Social },
            { DefaultSkills.Trade, DefaultCharacterAttributes.Social },

            { DefaultSkills.Steward, DefaultCharacterAttributes.Intelligence },
            { DefaultSkills.Medicine, DefaultCharacterAttributes.Intelligence },
            { DefaultSkills.Engineering, DefaultCharacterAttributes.Intelligence },
        };

    public static IEnumerable<CharacterAttribute> GetAttributesFromSkill(SkillObject skill)
    {
        switch (skill.StringId)
        {
            case "Mariner":
                yield return DefaultCharacterAttributes.Endurance;
                yield return DefaultCharacterAttributes.Cunning;
                yield break;

            case "Boatswain":
                yield return DefaultCharacterAttributes.Social;
                yield return DefaultCharacterAttributes.Control;
                yield break;

            case "Shipmaster":
                yield return DefaultCharacterAttributes.Vigor;
                yield return DefaultCharacterAttributes.Intelligence;
                yield break;
        }

        var attr = GetAttributeFromSkill(skill);
        if (attr != null)
            yield return attr;
    }

}