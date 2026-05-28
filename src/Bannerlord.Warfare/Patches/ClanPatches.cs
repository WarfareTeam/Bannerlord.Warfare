using TaleWorlds.CampaignSystem;

using HarmonyLib;

using Warfare.Behaviors;

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
    [HarmonyPatch(typeof(Clan), "SetLeader")]
    public static class SetLeaderPatch
    {
        public static void Postfix(Clan __instance)
        {
            // Name mercenary clans according to our rules on creation, for immersion in case mods spawn new minor clans.
            if (Campaign.Current.GameStarted && __instance.IsMinorFaction && __instance != Clan.PlayerClan)
            {
                SubModule.MercenaryBehavior.ChangeClanName(__instance);
            }
        }
    }
}
