using System.Collections.Generic;
using System.Linq;
using AutoAssignCharacterSheetWithTemplate.Models;
using TaleWorlds.CampaignSystem;

namespace AutoAssignCharacterSheetWithTemplate.Utils;

public static class TemplateHeroUtils
    {
        public static List<HeroSelectionItem> GetHeroSelectionEntries(IEnumerable<TemplateManagerCharacter> allTemplates, TemplateManagerCharacter currentTemplate)
        {
            var result = new List<HeroSelectionItem>();

            var clan = Clan.PlayerClan;
            if (clan == null) return result;

            var clanHeroes = clan.Heroes.Where(h => h.IsAlive).ToList();

            var heroToTemplate = new Dictionary<Hero, TemplateManagerCharacter>();
            foreach (var t in allTemplates)
            {
                if (t?.HeroList == null) continue;
                foreach (var h in t.HeroList)
                {
                    if (h == null) continue;
                    if (!heroToTemplate.ContainsKey(h))
                        heroToTemplate[h] = t;
                }
            }

            foreach (var hero in clanHeroes)
            {
                bool isSelected = currentTemplate != null && currentTemplate.HeroList.Contains(hero);
                bool inOther = heroToTemplate.TryGetValue(hero, out var ownerTemplate) && ownerTemplate != null && ownerTemplate != currentTemplate;
                bool isEnabled = !inOther || isSelected; // enabled if not in other template, or if it's selected in current

                result.Add(new HeroSelectionItem
                {
                    Hero = hero,
                    IsEnabled = isEnabled,
                    IsSelected = isSelected
                });
            }

            return result;
        }

        public static bool IsHeroInOtherTemplate(Hero hero, TemplateManagerCharacter currentTemplate, IEnumerable<TemplateManagerCharacter> allTemplates)
        {
            if (hero == null) return false;
            foreach (var t in allTemplates)
            {
                if (t == null) continue;
                if (t == currentTemplate) continue;
                if (t.HeroList != null && t.HeroList.Contains(hero)) return true;
            }
            return false;
        }
    }