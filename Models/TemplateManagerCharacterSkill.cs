namespace AutoAssignCharacterSheetWithTemplate.Models;

public class TemplateManagerCharacterSkill
{
    public string StringId { get; set; }
    public bool AreAllPerksSelected { get; set; }
    public bool IsSkillImportant { get; set; }
    
    public TemplateManagerCharacterSkill(string stringId, bool areAllPerksSelected, bool isSkillImportant)
    {
        StringId = stringId;
        AreAllPerksSelected = areAllPerksSelected;
        IsSkillImportant = isSkillImportant;
    }
}