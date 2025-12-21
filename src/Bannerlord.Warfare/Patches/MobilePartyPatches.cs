using TaleWorlds.CampaignSystem.Party;

using HarmonyLib;

using Warfare.Extensions;
using TaleWorlds.CampaignSystem;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(MobileParty), "InitializeMobilePartyWithPartyTemplate")]
    public static class InitializeMobilePartyWithPartyTemplatePatch
    {
        public static bool Prefix(MobileParty __instance)
        {
            //We manually add troops to each mercenary party roster in code to ensure compatibiility with overhauls and other mods changing spclans.xml without using xslt
            
            return __instance == null || !__instance.IsActive || !__instance.IsLordParty || !__instance.ActualClan.IsMinorFaction || __instance == MobileParty.MainParty;
        }
    }

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

