using TaleWorlds.CampaignSystem.Party;

using HarmonyLib;

using Warfare.Extensions;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(MobileParty), "FillPartyStacks")]
    public static class FillPartyStacksPatch
    {
        public static bool Prefix(MobileParty __instance)
        {
            //We manually add troops to each mercenary party roster in code to ensure compatibiility with overhauls and other mods changing spclans.xml without using xslt
            return __instance == null || !__instance.IsActive || !__instance.IsLordParty || !__instance.ActualClan.IsMinorFaction || __instance == MobileParty.MainParty;
        }
    }

    [HarmonyPatch(typeof(MobileParty), "LimitedPartySize", MethodType.Getter)]
    public static class LimitedPartySizePatch
    {
        public static void Postfix(MobileParty __instance, ref int __result)
        {
            if (__instance.ActualClan != null && __instance.IsLordParty && __instance.ActualClan.IsMinorFaction && __instance != MobileParty.MainParty)
            {
                __result = __instance.ActualClan.GetRosterLimit();
            }
        }
    }

    [HarmonyPatch(typeof(MobileParty), "HasLimitedWage")]
    public static class HasLimitedWagePatch
    {
        public static void Postfix(MobileParty __instance, ref bool __result)
        {
            if (__result && __instance.ActualClan != null && __instance.IsLordParty && __instance.ActualClan.IsMinorFaction && __instance != MobileParty.MainParty)
            {
                __result = false;
            }
        }
    }
}

