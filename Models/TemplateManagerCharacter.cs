using System.Collections.Generic;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Utils;
using Newtonsoft.Json;
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
    private bool _isFromNewCreatedTemplate;
    public TemplateCharacterDto SavedStateSnapshot { get; private set; }


    public TemplateManagerCharacter()
    {
        Name = string.Empty;
        SkillList = new List<TemplateManagerCharacterSkill>();
        PerkList = new List<TemplateManagerCharacterPerk>();
        HeroList = new List<Hero>();
        _isFromNewCreatedTemplate = false;
    }


    public void SetImportantSkill(SkillObject skill, bool isImportant)
    {
        TemplateManagerCharacterSkill? result = SkillList.FirstOrDefault((obj) => obj.StringId.Equals(skill.StringId));
        if (result != null)
        {
            result.IsSkillImportant = isImportant;
        }
    }

    public void SetImportantSkill(string skillId, bool isImportant)
    {
        TemplateManagerCharacterSkill? result = SkillList.FirstOrDefault(obj => obj.StringId.Equals(skillId));
        if (result != null)
        {
            result.IsSkillImportant = isImportant;
        }
    }

    public bool GetImportantSkill(SkillObject skill)
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
        if (result != null)
        {
            result.Enable = enable;
        }
        else
        {
            PerkList.Add(new TemplateManagerCharacterPerk(perk.StringId, perk.Skill.StringId, enable));
        }
    }

    public void SetPerkValue(string perkId, bool enable)
    {
        TemplateManagerCharacterPerk? result = PerkList.FirstOrDefault(cp => cp.StringId.Equals(perkId));
        if (result != null)
        {
            result.Enable = enable;
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

        var listSkills = CharacterUtils.GetSkillsWithWarSails();
        foreach (var skill in listSkills)
        {
            var pid = skill.StringId ?? skill.GetType().Name;
            var skillEntry = new TemplateManagerCharacterSkill(skill, pid, false);
            SkillList.Add(skillEntry);
        }
    }
    
    public bool GetIsFromNewCreatedTemplate()
    {
        return _isFromNewCreatedTemplate;
    }

    public void SetIsFromNewCreatedTemplate(bool newValue)
    {
        _isFromNewCreatedTemplate = newValue;
    }

    public void UpdateSavedStateSnapshot()
    {
        SavedStateSnapshot = TemplateCharacterDto.FromModel(this);
    }
    
    public bool HasUnsavedChanges()
    {
        if (SavedStateSnapshot == null)
            return true;

        var currentDto = TemplateCharacterDto.FromModel(this);
        
        string savedJson = JsonConvert.SerializeObject(SavedStateSnapshot);
        string currentJson = JsonConvert.SerializeObject(currentDto);

        return savedJson != currentJson;
    }
}