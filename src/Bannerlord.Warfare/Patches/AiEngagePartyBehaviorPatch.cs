using HarmonyLib;
using Helpers;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using Warfare;
using Warfare.Behaviors;
using Warfare.Content.Strategies;
using Warfare.Helpers;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(AiEngagePartyBehavior), "AiHourlyTick")]
    public static class AiEngagePartyBehaviorPatch
    {
        //TODO: Make a transpiler for the method
        public static bool Prefix(MobileParty mobileParty, PartyThinkParams p, IDisbandPartyCampaignBehavior ____disbandPartyCampaignBehavior)
        {
            if (!Settings.Current.EnableStrategies)
            {
                return true;
            }
            if ((mobileParty.CurrentSettlement != null && mobileParty.CurrentSettlement.SiegeEvent != null) || (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty && mobileParty.Army.ArmyType != Army.ArmyTypes.Defender))
            {
                return false;
            }

            float num = Campaign.Current.Models.EncounterModel.NeededMaximumDistanceForEncounteringMobileParty * 45f;
            if ((!mobileParty.MapFaction.IsKingdomFaction && mobileParty.MapFaction != Hero.MainHero.MapFaction) || mobileParty.IsCaravan || (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty) || mobileParty.LeaderHero == null)
            {
                return false;
            }

            bool flag = !mobileParty.MapFaction.Settlements.Any();
            float num2 = 0f;
            if (!flag)
            {
                float num3 = Campaign.MapDiagonalSquared;
                float averageDistanceBetweenClosestTwoTownsWithNavigationType = Campaign.Current.GetAverageDistanceBetweenClosestTwoTownsWithNavigationType(mobileParty.NavigationCapability);
                LocatableSearchData<Settlement> data = Settlement.StartFindingLocatablesAroundPosition(mobileParty.Position.ToVec2(), averageDistanceBetweenClosestTwoTownsWithNavigationType * 0.76f);
                for (Settlement settlement = Settlement.FindNextLocatable(ref data); settlement != null; settlement = Settlement.FindNextLocatable(ref data))
                {
                    if (settlement.MapFaction == mobileParty.MapFaction)
                    {
                        float num4 = settlement.Position.DistanceSquared(mobileParty.Position);
                        if (num4 < num3)
                        {
                            num3 = num4;
                        }
                    }
                }

                float num5 = MathF.Sqrt(num3);
                float num6 = Campaign.Current.EstimatedAverageLordPartySpeed * (float)CampaignTime.HoursInDay;
                num2 = ((num5 < num6 * 0.5f) ? (1f - MathF.Max(0f, num5 - num6 * 0.15f) / num6 * 0.3f) : 0f);
            }

            if (!flag && !(num2 > 0f))
            {
                return false;
            }

            float num7 = mobileParty.PartySizeRatio;
            foreach (MobileParty attachedParty in mobileParty.AttachedParties)
            {
                num7 += attachedParty.PartySizeRatio;
            }

            float num8 = MathF.Min(1f, num7 / ((float)mobileParty.AttachedParties.Count + 1f));
            float num9 = num8 * ((num8 <= 0.5f) ? num8 : (0.5f + 0.707f * MathF.Sqrt(num8 - 0.5f)));
            LocatableSearchData<MobileParty> data2 = MobileParty.StartFindingLocatablesAroundPosition(mobileParty.Position.ToVec2(), num);
            for (MobileParty mobileParty2 = MobileParty.FindNextLocatable(ref data2); mobileParty2 != null; mobileParty2 = MobileParty.FindNextLocatable(ref data2))
            {
                if (!mobileParty2.IsActive || (!mobileParty2.IsLordParty && mobileParty2.IsCurrentlyAtSea != mobileParty.IsCurrentlyAtSea))
                {
                    continue;
                }

                IFaction mapFaction = mobileParty2.MapFaction;
                if (mapFaction == null || !mapFaction.IsAtWarWith(mobileParty.MapFaction) || (mobileParty2.Army != null && mobileParty2 != mobileParty2.Army.LeaderParty))
                {
                    continue;
                }

                IFaction mapFaction2 = mobileParty2.MapFaction;
                if (((mapFaction2 == null || !mapFaction2.IsKingdomFaction) && mobileParty2.MapFaction != Hero.MainHero.MapFaction) || (mobileParty2.CurrentSettlement != null && mobileParty2.CurrentSettlement.IsFortification) || !(mobileParty2.Aggressiveness > 0.1f) || mobileParty2.ShouldBeIgnored)
                {
                    continue;
                }

                MobileParty.NavigationType bestNavigationType = MobileParty.NavigationType.None;
                float bestNavigationDistance = Campaign.MapDiagonal;
                bool flag2 = mobileParty.HasNavalNavigationCapability && mobileParty.CurrentSettlement != null && mobileParty.CurrentSettlement.HasPort && mobileParty2.IsCurrentlyAtSea;
                if (mobileParty.CurrentSettlement == null && mobileParty2.CurrentSettlement == null)
                {
                    AiHelper.GetBestNavigationTypeAndDistanceOfMobilePartyForMobileParty(mobileParty, mobileParty2, out bestNavigationType, out bestNavigationDistance);
                }
                else if (mobileParty2.CurrentSettlement == null)
                {
                    AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(mobileParty2, mobileParty.CurrentSettlement, isTargetingPort: false, out bestNavigationType, out bestNavigationDistance, out var _);
                }
                else
                {
                    bestNavigationType = ((!flag2) ? MobileParty.NavigationType.Default : MobileParty.NavigationType.Naval);
                    bestNavigationDistance = Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty2.CurrentSettlement, mobileParty.CurrentSettlement, flag2, flag2, bestNavigationType);
                }

                if (!(bestNavigationDistance < num))
                {
                    continue;
                }

                float num10 = 0f;
                num10 = ((mobileParty2.Army == null) ? mobileParty2.Party.EstimatedStrength : mobileParty2.Army.EstimatedStrength);
                float num11 = 1f - bestNavigationDistance / num;
                float num12 = 1f;
                if (mobileParty2.LeaderHero != null)
                {
                    int relation = mobileParty2.LeaderHero.GetRelation(mobileParty.LeaderHero);
                    num12 = ((relation >= 0) ? (1f - MathF.Sqrt(relation) / 10f) : (1f + MathF.Sqrt(-relation) / 20f));
                }

                float num13 = 0f;
                LocatableSearchData<MobileParty> data3 = MobileParty.StartFindingLocatablesAroundPosition(mobileParty.Position.ToVec2(), num);
                for (MobileParty mobileParty3 = MobileParty.FindNextLocatable(ref data3); mobileParty3 != null; mobileParty3 = MobileParty.FindNextLocatable(ref data3))
                {
                    if (mobileParty3 != mobileParty && mobileParty3.MapFaction == mobileParty.MapFaction && (mobileParty3.Army == null || mobileParty3.Army.LeaderParty == mobileParty3) && ((mobileParty3.DefaultBehavior == AiBehavior.GoAroundParty && mobileParty3.TargetParty == mobileParty2) || (mobileParty3.ShortTermBehavior == AiBehavior.EngageParty && mobileParty3.ShortTermTargetParty == mobileParty2)))
                    {
                        num13 += mobileParty3.Army?.EstimatedStrength ?? mobileParty3.Party.EstimatedStrength;
                    }
                }

                float num14 = 0f;
                num14 = mobileParty.Army?.EstimatedStrength ?? mobileParty.Party.EstimatedStrength;
                float num15 = (num13 + num14) / num10;
                float num16 = 0f;
                if (mobileParty.Army == null || mobileParty.Army.LeaderParty != mobileParty || !(num14 > num10 * 2f))
                {
                    float num17 = ((mobileParty2.CurrentSettlement != null && mobileParty2.CurrentSettlement.IsFortification && mobileParty2.CurrentSettlement.MapFaction != mobileParty.MapFaction) ? 0.25f : 1f);
                    float num18 = 1f;
                    if (num13 + (num14 + 30f) > num10 * 1.5f)
                    {
                        float num19 = num10 * 1.5f + 10f + ((mobileParty2.MapEvent != null || mobileParty2.SiegeEvent != null) ? 30f : 0f);
                        float num20 = num13 + (num14 + 30f);
                        num18 = MathF.Pow(num19 / num20, 0.8f);
                    }

                    float lastCalculatedSpeed = mobileParty.Speed;
                    float lastCalculatedSpeed2 = mobileParty2.Speed;
                    float num21 = lastCalculatedSpeed / lastCalculatedSpeed2;
                    float num22 = num21 * num21 * num21 * num21;
                    float num23 = ((lastCalculatedSpeed > lastCalculatedSpeed2 && mobileParty.Army == null) ? 1f : ((num13 + num14 > num10) ? (0.5f + 0.5f * num22 * num18) : (0.5f * num22)));
                    float num24 = ((mobileParty.DefaultBehavior == AiBehavior.GoAroundParty && mobileParty2 == mobileParty.TargetParty) ? 1.1f : 1f);
                    float num25 = ((mobileParty.Army != null) ? 0.9f : 1f);
                    float num26 = ((mobileParty2 == MobileParty.MainParty) ? 1.2f : 1f);
                    float num27 = 1f;
                    if (mobileParty.Objective == MobileParty.PartyObjective.Defensive)
                    {
                        num27 = 1.2f;
                    }

                    float num28 = 1f;
                    if (mobileParty.MapFaction != null && mobileParty.MapFaction.IsKingdomFaction && mobileParty.MapFaction.Leader == Hero.MainHero)
                    {
                        StanceLink stanceWith = Hero.MainHero.MapFaction.GetStanceWith(mobileParty2.MapFaction);
                        if (stanceWith != null && stanceWith.BehaviorPriority == 1)
                        {
                            num28 = 1.2f;
                        }
                        Strategy strategy = Campaign.Current.GetCampaignBehavior<StrategyBehavior>().FindStrategy(mobileParty.Owner);
                        if (strategy != null)
                        {
                            if (strategy.Priority == 1)
                            {
                                num28 *= Settings.Current.ChaseTendencyDefensiveStrategy;
                            }
                            else if (strategy.Priority == 2)
                            {
                                num28 *= Settings.Current.ChaseTendencyOffensiveStrategy;
                            }
                        }
                    }

                    num16 = num11 * num2 * num12 * num15 * num26 * num9 * num23 * num18 * num17 * num24 * num25 * num27 * num28 * 2f;
                }

                if (num16 > 0.05f && mobileParty2.CurrentSettlement == null)
                {
                    float averageDistanceBetweenClosestTwoTownsWithNavigationType2 = Campaign.Current.GetAverageDistanceBetweenClosestTwoTownsWithNavigationType(mobileParty.NavigationCapability);
                    float num29 = Campaign.MapDiagonalSquared;
                    LocatableSearchData<Settlement> data4 = Settlement.StartFindingLocatablesAroundPosition(mobileParty2.Position.ToVec2(), averageDistanceBetweenClosestTwoTownsWithNavigationType2 * 0.38f);
                    for (Settlement settlement2 = Settlement.FindNextLocatable(ref data4); settlement2 != null; settlement2 = Settlement.FindNextLocatable(ref data4))
                    {
                        if (settlement2.MapFaction == mobileParty2.MapFaction)
                        {
                            float num30 = settlement2.Position.DistanceSquared(mobileParty.Position);
                            if (num30 < num29)
                            {
                                num29 = num30;
                            }
                        }
                    }

                    if (num29 < averageDistanceBetweenClosestTwoTownsWithNavigationType2 * 9.6f)
                    {
                        float num31 = MathF.Sqrt(num29);
                        num16 *= 0.25f + 0.75f * (MathF.Max(0f, num31 - 5f) / 20f);
                        if (!mobileParty.IsDisbanding)
                        {
                            IDisbandPartyCampaignBehavior disbandPartyCampaignBehavior = ____disbandPartyCampaignBehavior;
                            if (disbandPartyCampaignBehavior == null || !disbandPartyCampaignBehavior.IsPartyWaitingForDisband(mobileParty))
                            {
                                goto IL_0857;
                            }
                        }

                        num16 *= 0.25f;
                    }
                }

                goto IL_0857;
            IL_0857:
                p.CurrentObjectiveValue = num16;
                AiBehavior aiBehavior = AiBehavior.GoAroundParty;
                AIBehaviorData item = new AIBehaviorData(mobileParty2, aiBehavior, bestNavigationType, willGatherArmy: false, flag2, isTargetingPort: false);
                (AIBehaviorData, float) value = (item, num16);
                p.AddBehaviorScore(in value);
            }
            return false;
        }
    }
}
