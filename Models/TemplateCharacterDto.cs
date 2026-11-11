using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace AutoAssignCharacterSheetWithTemplate.Models;

public class TemplateCharacterDto
{
    public string Name { get; set; }
    public List<string> ImportantSkillIdList { get; set; }
    public List<string> EnabledPerkIdList { get; set; }
    public List<string> HeroIdList { get; set; }

    public static TemplateCharacterDto FromModel(TemplateManagerCharacter model)
    {
        return new TemplateCharacterDto
        {
            Name = model.Name,
            ImportantSkillIdList = GetImportantSkillIdsFromList(model.SkillList),
            EnabledPerkIdList = GetEnabledPerkIdsFromList(model.PerkList),
            HeroIdList = GetHeroIdsFromList(model.HeroList),
        };
    }

    private static List<string> GetImportantSkillIdsFromList(List<TemplateManagerCharacterSkill> skillList)
    {
        var result = new List<string>();
        foreach (var skill in skillList)
        {
            if (skill.IsSkillImportant)
            {
                result.Add(skill.StringId);
            }
        }
        return result;
    }
    
    private static List<string> GetEnabledPerkIdsFromList(List<TemplateManagerCharacterPerk> perkList)
    {
        var result = new List<string>();
        foreach (var perk in perkList)
        {
            if (perk.Enable)
            {
                result.Add(perk.StringId);
            }
        }
        return result;
    }
    
    private static List<string> GetHeroIdsFromList(List<Hero> heroList)
    {
        var result = new List<string>();
        foreach (var hero in heroList)
        {
            result.Add(hero.StringId);
        }
        return result;
    }

    public void ApplyToModel(TemplateManagerCharacter model)
    {
        model.Name = Name;
        foreach (var skillId in ImportantSkillIdList)
        {
            model.SetImportantSkill(skillId, true);
        }

        foreach (var perkId in EnabledPerkIdList)
        {
            model.SetPerkValue(perkId, true);
        }

        foreach (var heroId in HeroIdList)
        {
            Hero? result = Hero.MainHero.Clan.Heroes.Find(x => x.StringId == heroId);
            if (result != null)
            {
                model.HeroList.Add(result);
            }
        }
    }
}