using TaleWorlds.CampaignSystem.Party;

using HarmonyLib;

using Warfare.Extensions;
using TaleWorlds.CampaignSystem;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(PartyBase), "PartySizeLimit", MethodType.Getter)]
    public static class PartySizeLimitPatch
    {
        public static void Postfix(PartyBase __instance, ref int __result)
        {
            MobileParty party = __instance.MobileParty;
            
            if (party.ActualClan != null && party.IsLordParty && party.ActualClan.IsMinorFaction && party.ActualClan != Clan.PlayerClan)
            {
                __result = party.ActualClan.GetRosterLimit();
            }
        }
    }

    [HarmonyPatch(typeof(MobileParty), "HasLimitedWage")]
    public static class HasLimitedWagePatch
    {
        public static void Postfix(MobileParty __instance, ref bool __result)
        {
            if (__result && __instance.ActualClan != null && __instance.IsLordParty && __instance.ActualClan.IsMinorFaction && __instance.ActualClan != Clan.PlayerClan)
            {
                __result = false;
            }
        }
    }
}

