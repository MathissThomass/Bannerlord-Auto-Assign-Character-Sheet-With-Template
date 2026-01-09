using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.Models;

public class TemplateManagerCharacter
{
    public string Name { get; set; }

    public List<TemplateManagerCharacterSkill> SkillList { get; set; }
    public List<TemplateManagerCharacterPerk> PerkList { get; set; }
    public List<Hero> HeroList { get; set; }


    public TemplateManagerCharacter()
    {
        Name = string.Empty;
        SkillList = new List<TemplateManagerCharacterSkill>();
        PerkList = new List<TemplateManagerCharacterPerk>();
        HeroList = new List<Hero>();
    }


    public void SetSkillImportantSkill(SkillObject skill, bool isImportant)
    {
        TemplateManagerCharacterSkill? result = SkillList.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
        if (result != null)
        {
            result.IsSkillImportant = isImportant;
        }
    }

    public bool GetSkillImportantSkill(SkillObject skill)
    {
        TemplateManagerCharacterSkill? result = SkillList.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
        if (result != null)
        {
            return result.IsSkillImportant;
        }
        else
        {
            return false;
        }
    }

    public void SetAreAllPerksSelectedFromSkill(SkillObject skill, bool areAllPerksSelected)
    {
        TemplateManagerCharacterSkill? result = SkillList.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
        if (result != null)
        {
            result.AreAllPerksSelected = areAllPerksSelected;
        }
    }

    public bool GetAreAllPerksSelectedFromSkill(SkillObject skill)
    {
        TemplateManagerCharacterSkill? result = SkillList.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));

        if (result != null)
        {
            return result.AreAllPerksSelected;
        }
        else
        {
            return true;
        }
    }

    public void ClearPerkSkill(SkillObject skill)
    {
        List<TemplateManagerCharacterPerk> results =
            PerkList.FindAll((obj) => obj.BelongToSkillStringId.Equals(skill.StringId));

        if (!results.IsEmpty())
        {
            foreach (TemplateManagerCharacterPerk perk in results)
            {
                perk.Enable = false;
            }
        }
    }

    public void ClearAllPerks()
    {
        foreach (TemplateManagerCharacterPerk perk in PerkList)
        {
            perk.Enable = false;
        }
    }

    public void SetPerkValue(PerkObject perk, bool enable)
    {
        TemplateManagerCharacterPerk? result = PerkList.FirstOrDefault(cp => cp.StringId.Equals(perk.StringId));
        if (null != result)
        {
            result.Enable = enable;
        }
        else
        {
            PerkList.Add(new TemplateManagerCharacterPerk(perk.StringId, perk.Skill.StringId, enable));
        }
    }

    public bool GetPerkValue(PerkObject perk)
    {
        return PerkList.Any(cp => cp.StringId.Equals(perk.StringId) && cp.Enable);
    }

    public void CreateNewTemplate()
    {
        var listPerks = PerkObject.All.OrderBy(p => p.RequiredSkillValue);
        var seenPerks = new HashSet<string>();
        foreach (var perk in listPerks)
        {
            if (perk == null)
            {
                continue;
            }

            var pid = perk.StringId ?? perk.GetType().Name;
            if (!seenPerks.Add(pid)) continue;
            var belongSkillId = perk.Skill?.StringId ?? string.Empty;
            var perkEntry = new TemplateManagerCharacterPerk(pid, belongSkillId, false);
            PerkList.Add(perkEntry);
        }
    }
}