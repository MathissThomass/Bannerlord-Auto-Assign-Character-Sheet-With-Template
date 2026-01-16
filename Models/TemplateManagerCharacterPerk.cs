namespace AutoAssignCharacterSheetWithTemplate.Models;

public class TemplateManagerCharacterPerk
{
    public string StringId { set; get; }

    public string BelongToSkillStringId { set; get; }

    public bool Enable { set; get; }

    public TemplateManagerCharacterPerk(string perkStringId, string belongToSkillStringId,  bool enable)
    {
        StringId = perkStringId;
        BelongToSkillStringId = belongToSkillStringId;
        Enable = enable;
    }
}