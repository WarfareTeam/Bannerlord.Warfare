using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace Warfare
{
    internal class Commands
    {
        [CommandLineFunctionality.CommandLineArgumentFunction("party_food", "campaign")]
        internal static string CheckPartyFood(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"campaign.party_food [HeroName]\".";
            }
            CampaignCheats.TryGetObject(strings[0], out Hero hero, out string str, (Hero x) => x.IsAlive);
            if (hero == null)
            {
                return "Hero is not found";
            }
            if (hero.PartyBelongedTo == null)
            {
                return "Hero: " + strings[0] + " is not in a party.";
            }
            foreach (MobileParty party in MobileParty.All)
            {
                if (string.Equals(hero.PartyBelongedTo.Name.ToString(), party.Name.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return party.Name.ToString() + " has " + party.TotalFoodAtInventory + " food remaining (" + party.GetNumDaysForFoodToLast() + " days).";
                }
            }
            return "Party is not found for hero: " + hero.Name.ToString();
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("hero_gold", "campaign")]
        internal static string CheckPartyGold(List<string> strings)
        {
            if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
            {
                return CampaignCheats.ErrorType;
            }
            if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
            {
                return "Format is \"campaign.hero_gold [HeroName]\".";
            }
            CampaignCheats.TryGetObject(strings[0], out Hero hero, out string str, (Hero x) => x.IsAlive);
            if (hero == null)
            {
                return "Hero is not found";
            }
            if (hero.PartyBelongedTo == null)
            {
                return "Hero: " + strings[0] + " is not in a party.";
            }
            foreach (MobileParty party in MobileParty.All)
            {
                if (string.Equals(hero.PartyBelongedTo.Name.ToString(), party.Name.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    int gold = party.LeaderHero.Gold;
                    return party.Name.ToString() + " has " + gold + " gold remaining with " + party.TotalWage + " daily wages.";
                }
            }
            return "Party is not found for hero: " + hero.Name.ToString();
        }
    }
}