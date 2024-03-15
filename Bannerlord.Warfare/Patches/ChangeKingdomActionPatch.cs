using TaleWorlds.CampaignSystem.Actions;

using HarmonyLib;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyInternal")]
    public static class ChangeKingdomActionPatch
    {
        public static bool Prefix(ChangeKingdomAction.ChangeKingdomActionDetail detail)
        {
            return detail != ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary;
        }
    }
}