using HeroPathAutoBuildAssignment.Data;
using HeroPathAutoBuildAssignment.Utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace HeroPathAutoBuildAssignment.Behaviors;

public class AutoAssignBehavior : CampaignBehaviorBase
{
    public override void RegisterEvents()
    {
        CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, OnHeroGainedSkill);
        CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, OnGameLoadFinished);
        CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnHeroDied);
    }

    public override void SyncData(IDataStore dataStore)
    {
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
            
            if (hero.HeroDeveloper.UnspentAttributePoints > 0)
            {
                AutoAssign.AutoAssignAttributPoint(hero, heroTemplate);
            }

            if (hero.HeroDeveloper.UnspentFocusPoints > 0)
            {
                AutoAssign.AutoAssignFocusPoint(hero, heroTemplate);
            }
        }
    }

    private static void OnGameLoadFinished()
    {
        TemplateStore.Instance.LoadFromDisk();
    }

    private static void OnHeroDied(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail deathDetail, bool showNotification)
    {
        if (victim.Clan != Clan.PlayerClan)
        {
            return;
        }
        TemplateSaveManager.RemoveHeroFromTemplate(victim);
    }
}