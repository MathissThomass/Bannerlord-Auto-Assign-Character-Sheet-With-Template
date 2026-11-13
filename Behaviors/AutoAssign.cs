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

                if (!HasRoomForFocusPoint(hero, template, skill))
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
        var attributesWithoutRoom = new HashSet<CharacterAttribute>();

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
            
            var pick = CharacterUtils.PickAttributByWeight(weightList, attributesWithoutRoom);

            if (pick == null)
            {
                break;
            }

            if (!HasRoomForAttributePoint(hero, template, pick))
            {
                attributesWithoutRoom.Add(pick);
                continue;
            }
            
            hero.HeroDeveloper.AddAttribute(pick, 1);
        }
    }

    public static void CatchUpAssignedHero(Hero hero, TemplateManagerCharacter template)
    {
        if (hero?.HeroDeveloper == null || template == null)
        {
            return;
        }

        foreach (var skill in CharacterUtils.GetSkillsWithWarSails())
        {
            if (skill == null)
            {
                continue;
            }

            AutoAssignPerkPoint(hero, template, skill, 0);
        }

        if (hero.HeroDeveloper.UnspentAttributePoints > 0)
        {
            AutoAssignAttributPoint(hero, template);
        }

        if (hero.HeroDeveloper.UnspentFocusPoints > 0)
        {
            AutoAssignFocusPoint(hero, template);
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
            hero.HeroDeveloper.AddPerk(ChoosePerkRespectingCaps(hero, perk));
        }

        var perkToRandomize = unlockedPerks.Except(perkInTemplateToAdd);
        foreach (var perk in perkToRandomize)
        {
            if (!hero.GetPerkValue(perk) && perk.AlternativePerk == null)
            {
                hero.HeroDeveloper.AddPerk(ChoosePerkRespectingCaps(hero, perk));
                continue;
            }

            if (hero.GetPerkValue(perk) || perk.AlternativePerk == null ||
                hero.GetPerkValue(perk.AlternativePerk)) continue;
            var rand = _rng.Next(2);

            var picked = rand == 0 ? perk : perk.AlternativePerk;
            hero.HeroDeveloper.AddPerk(ChoosePerkRespectingCaps(hero, picked));
        }
    }

    private static bool HasRoomForFocusPoint(Hero hero, TemplateManagerCharacter template, SkillObject skill)
    {
        var maxFocus = Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill;
        var currentFocus = hero.HeroDeveloper.GetFocus(skill);
        return currentFocus + 1 + CountPendingFocusBonuses(hero, template, skill) <= maxFocus;
    }

    private static bool HasRoomForAttributePoint(Hero hero, TemplateManagerCharacter template, CharacterAttribute attribute)
    {
        var maxAttribute = Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute;
        var currentAttribute = hero.GetAttributeValue(attribute);
        return currentAttribute + 1 + CountPendingAttributeBonuses(hero, template, attribute) <= maxAttribute;
    }

    private static int CountPendingFocusBonuses(Hero hero, TemplateManagerCharacter template, SkillObject skill)
    {
        if (skill != DefaultSkills.OneHanded && skill != DefaultSkills.TwoHanded)
        {
            return 0;
        }

        return IsTemplatePerkStillPending(hero, template, DefaultPerks.Crafting.WeaponMasterSmith) ? 1 : 0;
    }

    private static int CountPendingAttributeBonuses(Hero hero, TemplateManagerCharacter template, CharacterAttribute attribute)
    {
        var count = 0;
        foreach (var perk in EnumerateAttributeGrantingPerks(attribute))
        {
            if (IsTemplatePerkStillPending(hero, template, perk))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsTemplatePerkStillPending(Hero hero, TemplateManagerCharacter template, PerkObject? perk)
    {
        if (perk == null || !template.GetPerkValue(perk) || hero.GetPerkValue(perk))
        {
            return false;
        }

        var alternative = perk.AlternativePerk;
        return alternative == null || !hero.GetPerkValue(alternative);
    }

    private static PerkObject ChoosePerkRespectingCaps(Hero hero, PerkObject perk)
    {
        if (!GrantsBlockedBonus(hero, perk))
        {
            return perk;
        }

        var alternative = perk.AlternativePerk;
        if (alternative == null || GrantsSameBlockedBonus(hero, perk, alternative))
        {
            return perk;
        }

        return alternative;
    }

    private static bool GrantsBlockedBonus(Hero hero, PerkObject perk)
    {
        foreach (var bonus in GetPerkBonuses(perk))
        {
            if (IsBonusAtCap(hero, bonus))
            {
                return true;
            }
        }

        return false;
    }

    private static bool GrantsSameBlockedBonus(Hero hero, PerkObject chosen, PerkObject alternative)
    {
        foreach (var blocked in GetPerkBonuses(chosen))
        {
            if (!IsBonusAtCap(hero, blocked))
            {
                continue;
            }

            foreach (var other in GetPerkBonuses(alternative))
            {
                if (blocked.Attribute != null && blocked.Attribute == other.Attribute)
                {
                    return true;
                }

                if (blocked.Skill != null && blocked.Skill == other.Skill)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsBonusAtCap(Hero hero, PerkBonus bonus)
    {
        if (bonus.Attribute != null)
        {
            return hero.GetAttributeValue(bonus.Attribute) >=
                   Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute;
        }

        if (bonus.Skill != null)
        {
            return hero.HeroDeveloper.GetFocus(bonus.Skill) >=
                   Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill;
        }

        return false;
    }

    private static IEnumerable<PerkObject> EnumerateAttributeGrantingPerks(CharacterAttribute attribute)
    {
        if (attribute == DefaultCharacterAttributes.Vigor)
        {
            yield return DefaultPerks.Crafting.VigorousSmith;
            yield return DefaultPerks.Athletics.Strong;
        }
        else if (attribute == DefaultCharacterAttributes.Control)
        {
            yield return DefaultPerks.Crafting.StrongSmith;
            yield return DefaultPerks.Athletics.Steady;
        }
        else if (attribute == DefaultCharacterAttributes.Endurance)
        {
            yield return DefaultPerks.Crafting.EnduringSmith;
            yield return DefaultPerks.Athletics.Durable;
        }
    }

    private static IEnumerable<PerkBonus> GetPerkBonuses(PerkObject perk)
    {
        if (perk == DefaultPerks.Crafting.VigorousSmith || perk == DefaultPerks.Athletics.Strong)
        {
            yield return new PerkBonus(DefaultCharacterAttributes.Vigor, null);
        }
        else if (perk == DefaultPerks.Crafting.StrongSmith || perk == DefaultPerks.Athletics.Steady)
        {
            yield return new PerkBonus(DefaultCharacterAttributes.Control, null);
        }
        else if (perk == DefaultPerks.Crafting.EnduringSmith || perk == DefaultPerks.Athletics.Durable)
        {
            yield return new PerkBonus(DefaultCharacterAttributes.Endurance, null);
        }
        else if (perk == DefaultPerks.Crafting.WeaponMasterSmith)
        {
            yield return new PerkBonus(null, DefaultSkills.OneHanded);
            yield return new PerkBonus(null, DefaultSkills.TwoHanded);
        }
    }

    private readonly struct PerkBonus
    {
        public PerkBonus(CharacterAttribute? attribute, SkillObject? skill)
        {
            Attribute = attribute;
            Skill = skill;
        }

        public CharacterAttribute? Attribute { get; }
        public SkillObject? Skill { get; }
    }
}