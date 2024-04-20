using TaleWorlds.CampaignSystem.Actions;

using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyInternal")]
    public static class ChangeKingdomActionPatch
    {
        public static bool Prefix(Clan clan, ChangeKingdomAction.ChangeKingdomActionDetail detail)
        {
            return clan == null || clan == Clan.PlayerClan || detail != ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary;
        }
    }
}