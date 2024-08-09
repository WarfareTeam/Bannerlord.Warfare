using HarmonyLib;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomWarDecision")]
    public static class GetRandomWarDecisionPatch
    {
        public static bool Prefix(KingdomDecision __result, Clan clan)
        {
            if (Settings.Current.ModifyWinterWarfareTendency && CampaignTime.Now.GetSeasonOfYear == CampaignTime.Seasons.Winter)
            {
                int proposerTrait = clan.Leader.GetTraitLevel(DefaultTraits.Calculating);
                int rulerTrait = clan.Kingdom.Leader.GetTraitLevel(DefaultTraits.Calculating);
                float proposerTraitScore = proposerTrait < 1 ? 0f : proposerTrait / 40f;
                float rulerTraitScore = rulerTrait < 1 ? 0f : rulerTrait / (clan.Leader != clan.Kingdom.Leader ? 40f : 20f);
                float declareWarScore = proposerTraitScore + rulerTraitScore;
                if (MBRandom.RandomFloat > declareWarScore)
                {
                    __result = null!;
                    return false;
                }
            }
            return true;
        }
    }
}
