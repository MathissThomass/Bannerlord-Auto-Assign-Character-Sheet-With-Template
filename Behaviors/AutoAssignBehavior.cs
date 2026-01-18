using AutoAssignCharacterSheetWithTemplate.Data;
using AutoAssignCharacterSheetWithTemplate.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace AutoAssignCharacterSheetWithTemplate.Behaviors;

public class AutoAssignBehavior : CampaignBehaviorBase
{
    public override void RegisterEvents()
    {
        CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, OnHeroLevelledUp);
        CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
        CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, OnGameLoadFinished);
    }

    public override void SyncData(IDataStore dataStore)
    {
    }

    private static void OnHeroLevelledUp(Hero hero, bool shouldNotify = true)
    {
        if (hero.Clan == Clan.PlayerClan)
        {
            var templateList = TemplateStore.Instance.TemplateList;
            var heroTemplate = CharacterUtils.FindHeroAssignedTemplate(hero, templateList);

            if (heroTemplate == null)
            {
                return;
            }

            if (hero.HeroDeveloper.UnspentAttributePoints > 0)
            {
                AutoAssign.AutoAssignAttributPoint(hero, heroTemplate);
            }

            AutoAssign.AutoAssignFocusPoint(hero, heroTemplate);
        }
    }

    private static void OnHeroGainedSkill(Hero hero, SkillObject skill, int change = 1, bool shouldNotify = true)
    {
        if (hero.Clan == Clan.PlayerClan)
        {
            var templateList = TemplateStore.Instance.TemplateList;
            var heroTemplate = CharacterUtils.FindHeroAssignedTemplate(hero, templateList);

            if (heroTemplate == null)
            {
                return;
            }

            var skillLvl = hero.GetSkillValue(skill);

            var remainder = skillLvl % 25;
            bool isPerkGained = remainder <= change || remainder == 0;
            if (isPerkGained)
            {
                var initialLvl = skillLvl - change;
                AutoAssign.AutoAssignPerkPoint(hero, heroTemplate, skill, initialLvl);
            }
        }
    }

    private static void OnGameLoadFinished()
    {
        TemplateStore.Instance.LoadFromDisk();
    }
}