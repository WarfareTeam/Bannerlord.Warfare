using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

using HarmonyLib;

using Warfare.Behaviors;
using Warfare.Prisoners;

namespace Bannerlord.Warfare.Patches
{
    /*
    [HarmonyPatch]
    internal class LootCollectorPatch
    {
        [HarmonyTargetMethod]
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.TypeByName("LootCollector"), "GiveShareOfLootToParty", null, null);
        }

        [HarmonyPostfix]
        private static void Postfix(TroopRoster prisonerRoster, PartyBase winnerParty)
        {
            Campaign.Current.GetCampaignBehavior<PrisonerBehavior>().CheckPotentialPrisoners(prisonerRoster, winnerParty);
        }
    }
    */
}
