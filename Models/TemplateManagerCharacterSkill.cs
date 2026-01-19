using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.Models;

public class TemplateManagerCharacterSkill
{
    public string StringId { get; set; }
    public bool IsSkillImportant { get; set; }
    
    public SkillObject Skill { get; set; }
    
    public TemplateManagerCharacterSkill(SkillObject skillObject, string stringId, bool isSkillImportant)
    {
        Skill = skillObject;
        StringId = stringId;
        IsSkillImportant = isSkillImportant;
    }
}