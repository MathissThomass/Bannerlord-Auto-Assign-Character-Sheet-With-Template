namespace AutoAssignCharacterSheetWithTemplate.Models;

public class TemplateManagerCharacterSkill
{
    public string StringId { get; set; }
    public bool IsSkillImportant { get; set; }
    
    public TemplateManagerCharacterSkill(string stringId, bool isSkillImportant)
    {
        StringId = stringId;
        IsSkillImportant = isSkillImportant;
    }
}