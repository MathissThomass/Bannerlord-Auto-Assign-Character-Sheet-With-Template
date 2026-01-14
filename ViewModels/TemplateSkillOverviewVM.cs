using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace AutoAssignCharacterSheetWithTemplate.ViewModels;

public class TemplateSkillOverviewVM : ViewModel
{
    private bool _isImportantSkill;
    private string _skillId;
    private SkillObject _skill;
    private BasicTooltipViewModel _hint;

    public TemplateSkillOverviewVM(SkillObject skillObject, bool isImportantSkill)
    {
        _isImportantSkill = isImportantSkill;
        _skillId = skillObject.StringId;
        _skill = skillObject;
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        string name = _skill.Name.ToString();
        string desc = _skill.Description.ToString();
        Hint = new BasicTooltipViewModel(delegate()
        {
            GameTexts.SetVariable("STR1", name);
            GameTexts.SetVariable("STR2", desc);
            return GameTexts.FindText("str_string_newline_string", null).ToString();
        });
    }

    [DataSourceProperty]
    public bool IsImportantSkill
    {
        get { return _isImportantSkill; }
        set
        {
            if (value != _isImportantSkill)
            {
                _isImportantSkill = value;
                OnPropertyChangedWithValue(value, "IsImportantSkill");
            }
        }
    }

    [DataSourceProperty]
    public string SkillId
    {
        get { return _skillId; }
        set
        {
            if (value != _skillId)
            {
                _skillId = value;
                OnPropertyChangedWithValue(value, "SkillId");
            }
        }
    }

    [DataSourceProperty]
    public BasicTooltipViewModel Hint
    {
        get { return _hint; }
        set
        {
            if (value != _hint)
            {
                _hint = value;
                OnPropertyChangedWithValue(value, "Hint");
            }
        }
    }
}