using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.CampaignSystem.Party;

using HarmonyLib;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanJoinAsMercenary")]
    public static class ConsiderClanJoinAsMercenaryPatch
    {
        public static bool Prefix()
        {
            //Kingdoms decide when to hire mercenaries with this mod.
            return false;
        }
    }
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanLeaveAsMercenary")]
    public static class ConsiderClanLeaveAsMercenaryPatch
    {
        public static bool Prefix()
        {
            //Mercenaries do not defect while under contract with this mod.
            return false;
        }
    }
    [HarmonyPatch(typeof(OutlawClansCampaignBehavior), "MakeOutlawFactionsEnemyToKingdomFactions")]
    public static class OutlawClansCampaignBehaviorPatch
    {
        public static bool Prefix()
        {
            //Mercenaries do not go to war independently with this mod.
            return false;
        }
    }
    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "CheckRecruiting")]
    public static class RecruitmentCampaignBehaviorPatch
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            // Mercenary troop recruiting is handled in MercenaryBehavior with this mod.
            return !mobileParty.IsLordParty || !mobileParty.ActualClan.IsMinorFaction || mobileParty == MobileParty.MainParty;
        }
    }
}
