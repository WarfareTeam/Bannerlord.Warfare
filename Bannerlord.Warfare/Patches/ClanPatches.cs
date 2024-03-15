using TaleWorlds.CampaignSystem;

using HarmonyLib;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(Clan), "UpdateBannerColorsAccordingToKingdom")]
    public static class UpdateBannerColorsAccordingToKingdomPatch
    {
        public static bool Prefix(Clan __instance)
        {
            // Do not update non-player mercenary clan banner colors on kingdom change.
            return __instance.Kingdom == null || !__instance.IsMinorFaction || __instance == Clan.PlayerClan;
        }
    }
}
