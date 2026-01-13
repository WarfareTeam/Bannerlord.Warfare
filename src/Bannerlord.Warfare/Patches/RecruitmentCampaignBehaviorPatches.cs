using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.LinQuick;

using HarmonyLib;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "CheckRecruiting")]
    public static class CheckRecruitingPatch
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (!mobileParty.IsCaravan)
            {
                if (!mobileParty.IsLordParty || mobileParty.IsDisbanding || mobileParty.LeaderHero == null || mobileParty.Party.IsStarving || !mobileParty.Party.LeaderHero.IsAlive || !(mobileParty.PartyTradeGold > HeroHelper.StartRecruitingMoneyLimit(mobileParty.LeaderHero)) || (mobileParty.LeaderHero != mobileParty.LeaderHero.Clan.Leader && !(mobileParty.LeaderHero.Clan.Gold > HeroHelper.StartRecruitingMoneyLimitForClanLeader(mobileParty.LeaderHero))) || !((mobileParty.Party.NumberOfAllMembers + 0.5f) / mobileParty.Party.PartySizeLimit <= 1f))
                {
                    return false;
                }
                if (mobileParty.Party.NumberOfAllMembers < mobileParty.Party.PartySizeLimit && !mobileParty.IsWageLimitExceeded())
                {
                    if (mobileParty.PartySizeRatio > 0.4f && mobileParty.ActualClan.MapFaction is Kingdom kingdom)
                    {
                        if (FactionHelper.GetStances(kingdom).WhereQ(y => y.IsAtWar && y.Faction1 is Kingdom && y.Faction2 is Kingdom).CountQ() > 0 && mobileParty.Party.NumberOfAllMembers / mobileParty.Party.PartySizeLimit >= mobileParty.TotalWage / mobileParty.PaymentLimit)
                        {
                            return false;
                        }

                    }
                }
            }
            return true;
        }
    }
}