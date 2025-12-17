using Helpers;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using Warfare;
using Warfare.Behaviors;
using Warfare.Content.Strategies;

namespace Warfare.Models
{
    public class WarfareTargetScoreCalculatingModel : TargetScoreCalculatingModel
    {
        private readonly TargetScoreCalculatingModel? _model;

        public WarfareTargetScoreCalculatingModel(TargetScoreCalculatingModel? model)
        {
            _model = model;
        }

        //TODO: Make a transpiler for the method

        public override float GetTargetScoreForFaction(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, float ourStrength)
        {
            if (!Settings.Current.EnableStrategies)
            {
                return _model.GetTargetScoreForFaction(targetSettlement, missionType, mobileParty, ourStrength);
            }
            IFaction mapFaction = mobileParty.MapFaction;
            AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(mobileParty, targetSettlement, isTargetingPort: false, out var _, out var bestNavigationDistance, out var _);
            float num = 0f;
            if (missionType == Army.ArmyTypes.Defender)
            {
                float num2 = 0f;
                float num3 = 0f;
                foreach (WarPartyComponent warPartyComponent in mapFaction.WarPartyComponents)
                {
                    MobileParty mobileParty2 = warPartyComponent.MobileParty;
                    if (mobileParty2 == mobileParty || (mobileParty2.Army != null && mobileParty2.Army == mobileParty.Army) || mobileParty2.AttachedTo != null)
                    {
                        continue;
                    }

                    if (mobileParty2.Army != null)
                    {
                        Army army = mobileParty2.Army;
                        if ((army.IsWaitingForArmyMembers() && army.AiBehaviorObject == targetSettlement) || (!army.IsWaitingForArmyMembers() && army.LeaderParty.DefaultBehavior == AiBehavior.DefendSettlement && army.AiBehaviorObject == targetSettlement) || (army.LeaderParty.TargetParty != null && (army.LeaderParty.TargetParty == targetSettlement.LastAttackerParty || (army.LeaderParty.TargetParty.MapEvent != null && army.LeaderParty.TargetParty.MapEvent == targetSettlement.LastAttackerParty.MapEvent) || (army.LeaderParty.TargetParty.BesiegedSettlement != null && army.LeaderParty.TargetParty.BesiegedSettlement == targetSettlement.LastAttackerParty.BesiegedSettlement))))
                        {
                            num3 += army.EstimatedStrength;
                        }
                    }
                    else if ((mobileParty2.DefaultBehavior == AiBehavior.DefendSettlement && mobileParty2.TargetSettlement == targetSettlement) || (mobileParty2.TargetParty != null && (mobileParty2.TargetParty == targetSettlement.LastAttackerParty || (mobileParty2.TargetParty.MapEvent != null && mobileParty2.TargetParty.MapEvent == targetSettlement.LastAttackerParty.MapEvent) || (mobileParty2.TargetParty.BesiegedSettlement != null && mobileParty2.TargetParty.BesiegedSettlement == targetSettlement.LastAttackerParty.BesiegedSettlement))))
                    {
                        num3 += mobileParty2.Party.EstimatedStrength;
                    }
                }

                float num4 = 0f;
                MobileParty lastAttackerParty = targetSettlement.LastAttackerParty;
                if (lastAttackerParty != null)
                {
                    if ((lastAttackerParty.MapEvent != null && lastAttackerParty.MapEvent.MapEventSettlement == targetSettlement) || lastAttackerParty.BesiegedSettlement == targetSettlement)
                    {
                        LocatableSearchData<MobileParty> data = MobileParty.StartFindingLocatablesAroundPosition(targetSettlement.GatePosition.ToVec2(), Campaign.Current.Models.EncounterModel.GetEncounterJoiningRadius * 3f);
                        for (MobileParty mobileParty3 = MobileParty.FindNextLocatable(ref data); mobileParty3 != null; mobileParty3 = MobileParty.FindNextLocatable(ref data))
                        {
                            if (mobileParty3.Aggressiveness > 0f && mobileParty3.MapFaction == lastAttackerParty.MapFaction)
                            {
                                num2 += ((mobileParty3.Aggressiveness > 0.5f) ? 1f : (mobileParty3.Aggressiveness * 2f)) * mobileParty3.Party.EstimatedStrength;
                            }
                        }
                    }
                    else
                    {
                        num2 = lastAttackerParty.Army?.EstimatedStrength ?? lastAttackerParty.Party.EstimatedStrength;
                    }
                }

                float num5 = ourStrength + num3;
                num4 = MathF.Max(100f, num2) * 1.1f;
                num = num5 / num2;
                if (num5 < num4)
                {
                    num *= 0.9f;
                }

                if (num5 > num4 * 1.75f)
                {
                    num *= 0.25f;
                }

                if (ourStrength < num2)
                {
                    num *= MathF.Pow(ourStrength / num2, 0.25f);
                }
            }
            else
            {
                float num6 = 0f;
                float num7 = 0f;
                bool flag = Hero.MainHero.CurrentSettlement == targetSettlement;
                foreach (MobileParty party in targetSettlement.Parties)
                {
                    if (party.Aggressiveness > 0.01f || party.IsGarrison || party.IsMilitia)
                    {
                        float num8 = ((party == MobileParty.MainParty) ? 0.5f : ((party.Army != null && party.Army.LeaderParty == MobileParty.MainParty) ? 0.8f : 1f));
                        float num9 = (flag ? 0.8f : 1f);
                        num6 += num8 * num9 * party.Party.EstimatedStrength;
                        if (!party.IsGarrison && !party.IsMilitia && party.LeaderHero != null)
                        {
                            num7 += num8 * num9 * party.Party.EstimatedStrength;
                        }
                    }
                }

                float num10 = 0f;
                float num11 = 0f;
                num11 = ((missionType != 0 || mobileParty.BesiegedSettlement == targetSettlement) ? 1f : (targetSettlement.IsTown ? 4f : 3f));
                float num12 = 0.7f;
                num11 *= 1f - 0.6f * (1f - num12) * (1f - num12);
                if (num6 < 100f && missionType == Army.ArmyTypes.Besieger)
                {
                    num11 *= 0.5f + 0.5f * (num6 / 100f);
                }

                if (missionType == Army.ArmyTypes.Raider)
                {
                    num11 *= 0.66f;
                }

                if ((mobileParty.MapEvent == null || mobileParty.MapEvent.MapEventSettlement != targetSettlement) && targetSettlement.MapFaction.IsKingdomFaction)
                {
                    int count = targetSettlement.MapFaction.Settlements.Count;
                    float b = (targetSettlement.MapFaction.CurrentTotalStrength * 0.25f - num6 - num7) / ((float)count + 10f);
                    num10 = MathF.Max(0f, b) * num11;
                }

                float num13 = ((missionType == Army.ArmyTypes.Besieger) ? (1f + 0.33f * (float)targetSettlement.Town.GetWallLevel()) : 1f);
                if (missionType == Army.ArmyTypes.Besieger && targetSettlement.Town.FoodStocks < 100f)
                {
                    num13 -= 0.5f * (num13 - 1f) * ((100f - targetSettlement.Town.FoodStocks) / 100f);
                }

                float num14 = ((missionType == Army.ArmyTypes.Besieger && mobileParty.LeaderHero != null) ? (mobileParty.LeaderHero.RandomFloat(0.1f) + (MathF.Max(MathF.Min(1.2f, mobileParty.Aggressiveness), 0.8f) - 0.8f) * 0.5f) : 0f);
                float num15 = num6 * (num13 - num14) + num10 + 0.1f;
                if (targetSettlement.SiegeEvent == null)
                {
                    float num16 = ((missionType == Army.ArmyTypes.Besieger) ? 2f : 0.75f);
                    if (mobileParty.SiegeEvent != null && mobileParty.BesiegedSettlement == targetSettlement)
                    {
                        num16 = 1.5f;
                    }

                    if (ourStrength < num15 * num16)
                    {
                        return 0f;
                    }
                }

                float num17 = 0f;
                if (missionType == Army.ArmyTypes.Besieger || (missionType == Army.ArmyTypes.Raider && targetSettlement.Party.MapEvent != null))
                {
                    float num18 = ((missionType == Army.ArmyTypes.Besieger) ? (Campaign.Current.EstimatedAverageLordPartySpeed * 2f) : Campaign.Current.EstimatedAverageLordPartySpeed);
                    float averageDistanceBetweenClosestTwoTownsWithNavigationType = Campaign.Current.GetAverageDistanceBetweenClosestTwoTownsWithNavigationType(MobileParty.NavigationType.All);
                    if (bestNavigationDistance < Math.Max(averageDistanceBetweenClosestTwoTownsWithNavigationType, num18))
                    {
                        LocatableSearchData<MobileParty> data2 = MobileParty.StartFindingLocatablesAroundPosition(targetSettlement.GatePosition.ToVec2(), num18);
                        for (MobileParty mobileParty4 = MobileParty.FindNextLocatable(ref data2); mobileParty4 != null; mobileParty4 = MobileParty.FindNextLocatable(ref data2))
                        {
                            if (mobileParty4.CurrentSettlement != targetSettlement && mobileParty4.Aggressiveness > 0.01f && mobileParty4.MapFaction == targetSettlement.Party.MapFaction && (!mobileParty4.IsMainParty || !mobileParty4.ShouldBeIgnored))
                            {
                                float num19 = ((mobileParty4 == MobileParty.MainParty || (mobileParty4.Army != null && mobileParty4.Army.LeaderParty == MobileParty.MainParty)) ? 0.5f : 1f);
                                float num20 = float.MaxValue;
                                num20 = ((mobileParty4.CurrentSettlement == null) ? Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty4, targetSettlement, isTargetingPort: false, MobileParty.NavigationType.All, out var _) : Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty4.CurrentSettlement, targetSettlement, isFromPort: false, isTargetingPort: false, MobileParty.NavigationType.All));
                                float value = num18 / (num20 + 0.001f);
                                value = MBMath.ClampFloat(value, 0.3f, 1.2f);
                                num17 += value * mobileParty4.Party.EstimatedStrength * num19;
                            }
                        }
                    }

                    if (num17 < ourStrength)
                    {
                        num17 = MathF.Max(0f, num17 - ourStrength * 0.33f);
                    }

                    num15 += num17;
                    num15 -= num10;
                    if (targetSettlement.MapFaction.IsKingdomFaction)
                    {
                        int count2 = targetSettlement.MapFaction.Settlements.Count;
                        float b2 = (targetSettlement.MapFaction.CurrentTotalStrength * 0.5f - (num7 + num17)) / ((float)count2 + 10f);
                        num10 = MathF.Max(0f, b2) * num11;
                    }

                    num15 += num10;
                }

                num = ourStrength / num15;
            }

            num = ((num > 2f) ? 2f : num);
            float averageDistanceBetweenClosestTwoTownsWithNavigationType2 = Campaign.Current.GetAverageDistanceBetweenClosestTwoTownsWithNavigationType(MobileParty.NavigationType.All);
            float num21 = MBMath.Map((5f * averageDistanceBetweenClosestTwoTownsWithNavigationType2 - bestNavigationDistance) / averageDistanceBetweenClosestTwoTownsWithNavigationType2, 0f, 5f, 0.9f, (missionType == Army.ArmyTypes.Raider) ? 3f : 10f);
            float num22 = ((missionType == Army.ArmyTypes.Raider) ? targetSettlement.GetSettlementValueForEnemyHero(mobileParty.LeaderHero) : targetSettlement.GetSettlementValueForFaction(mapFaction));
            float num23 = MathF.Pow(y: targetSettlement.IsVillage ? 0.5f : 0.33f, x: num22 / 50000f);
            float num24 = 1f;
            if (missionType == Army.ArmyTypes.Raider)
            {
                if (targetSettlement.Village.Bound.Town.FoodStocks < 100f)
                {
                    num23 *= 1f + 0.3f * ((100f - targetSettlement.Village.Bound.Town.FoodStocks) / 100f);
                }

                num23 *= 1.5f;
                num24 += ((mobileParty.Army != null) ? 0.5f : 1f) * ((mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan != null && mobileParty.LeaderHero.Clan.Gold < 10000) ? ((10000f - (float)mobileParty.LeaderHero.Clan.Gold) / 20000f) : 0f);
            }

            float num25 = missionType switch
            {
                Army.ArmyTypes.Besieger => 0.8f,
                Army.ArmyTypes.Defender => targetSettlement.IsVillage ? 1.28f : 1.75f,
                _ => 0.875f * (1f + (1f - targetSettlement.SettlementHitPoints)),
            };
            if (missionType == Army.ArmyTypes.Defender && targetSettlement.LastAttackerParty != null && ((targetSettlement.IsFortification && targetSettlement.LastAttackerParty.BesiegedSettlement != targetSettlement) || (!targetSettlement.IsFortification && targetSettlement.LastAttackerParty.MapEvent == null)))
            {
                MobileParty lastAttackerParty2 = targetSettlement.LastAttackerParty;
                MobileParty.NavigationType navCapabilities = ((!lastAttackerParty2.HasNavalNavigationCapability) ? MobileParty.NavigationType.Default : MobileParty.NavigationType.All);
                float b3 = DistanceHelper.FindClosestDistanceFromMobilePartyToSettlement(lastAttackerParty2, targetSettlement, navCapabilities) / (Campaign.Current.Models.PartySpeedCalculatingModel.CalculateFinalSpeed(lastAttackerParty2, Campaign.Current.Models.PartySpeedCalculatingModel.CalculateBaseSpeed(lastAttackerParty2, includeDescriptions: true)).ResultNumber * (float)CampaignTime.HoursInDay);
                float num26 = MathF.Min(0.5f, b3) / 0.5f;
                num25 = num26 * 0.8f + (1f - num26) * num25;
            }

            float num27 = 1f;
            if ((missionType == Army.ArmyTypes.Raider || missionType == Army.ArmyTypes.Besieger) && targetSettlement.OwnerClan != null && mobileParty.LeaderHero != null)
            {
                int relationWithClan = mobileParty.LeaderHero.Clan.GetRelationWithClan(targetSettlement.OwnerClan);
                if (relationWithClan > 0)
                {
                    num27 = 1f - ((missionType == Army.ArmyTypes.Besieger) ? 0.4f : 0.8f) * (MathF.Sqrt(relationWithClan) / 10f);
                }
                else if (relationWithClan < 0)
                {
                    num27 = 1f + ((missionType == Army.ArmyTypes.Besieger) ? 0.1f : 0.05f) * (MathF.Sqrt(-relationWithClan) / 10f);
                }
            }

            float num28 = 1f;
            if (mobileParty.MapFaction != null && mobileParty.MapFaction.IsKingdomFaction && mobileParty.MapFaction.Leader == Hero.MainHero && (missionType != Army.ArmyTypes.Defender || (targetSettlement.LastAttackerParty != null && targetSettlement.LastAttackerParty.MapFaction != Hero.MainHero.MapFaction)))
            {
                StanceLink stanceLink = ((missionType != Army.ArmyTypes.Defender) ? Hero.MainHero.MapFaction.GetStanceWith(targetSettlement.MapFaction) : Hero.MainHero.MapFaction.GetStanceWith(targetSettlement.LastAttackerParty.MapFaction));
                Strategy strategy = Campaign.Current.GetCampaignBehavior<StrategyBehavior>().FindStrategy(mobileParty.Owner);
                if (strategy != null)
                {
                    if (strategy.Priority == 1)
                    {
                        if (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider)
                        {
                            num28 *= Settings.Current.OffensiveTendencyDefensiveStrategy;
                        }
                        else if (missionType == Army.ArmyTypes.Defender)
                        {
                            num28 *= Settings.Current.DefensiveTendencyDefensiveStrategy;
                        }
                    }
                    else if (strategy.Priority == 2)
                    {
                        if (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider)
                        {
                            num28 *= Settings.Current.OffensiveTendencyOffensiveStrategy;
                        }
                        else if (missionType == Army.ArmyTypes.Defender)
                        {
                            num28 *= Settings.Current.DefensiveTendencyOffensiveStrategy;
                        }
                    }
                }
                if (stanceLink != null)
                {
                    if (stanceLink.BehaviorPriority == 1)
                    {
                        switch (missionType)
                        {
                            case Army.ArmyTypes.Besieger:
                            case Army.ArmyTypes.Raider:
                                num28 = 0.65f;
                                break;
                            case Army.ArmyTypes.Defender:
                                num28 = 1.1f;
                                break;
                        }
                    }
                    else if (stanceLink.BehaviorPriority == 2 && (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider))
                    {
                        num28 *= 1.3f;
                    }
                }
            }

            float num29 = 1f;
            if (mobileParty.TargetSettlement == targetSettlement)
            {
                num29 = 1.3f;
                if (mobileParty.SiegeEvent != null && mobileParty.SiegeEvent.BesiegedSettlement == targetSettlement)
                {
                    num29 = 4f;
                }

                if (mobileParty.MapEvent != null && mobileParty.MapEvent.IsRaid && mobileParty.MapEvent.MapEventSettlement == targetSettlement)
                {
                    num29 = 1.5f;
                }
            }

            float num30 = 1f;
            if (mobileParty.SiegeEvent == null && targetSettlement.SiegeEvent != null && targetSettlement.SiegeEvent.BesiegerCamp.MapFaction == mobileParty.MapFaction)
            {
                float num31 = targetSettlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType().Sum((PartyBase x) => x.EstimatedStrength);
                float num32 = targetSettlement.GetInvolvedPartiesForEventType().Sum((PartyBase x) => x.EstimatedStrength);
                if (num32 > 0f)
                {
                    float num33 = num31 / num32 - 5f;
                    if (num33 > 0f)
                    {
                        num33 = MBMath.ClampFloat(num33, 0f, 1f);
                        num30 = MBMath.Map(num33, 0f, 1f, 1f, 0f);
                    }
                }
                else
                {
                    num30 = 0f;
                }
            }

            float num34 = num27 * num * num23 * num24 * num25 * num28 * num29 * num30 * num21;
            if (mobileParty.Objective == MobileParty.PartyObjective.Defensive && missionType == Army.ArmyTypes.Defender)
            {
                num34 *= 1.2f;
            }
            else if (mobileParty.Objective == MobileParty.PartyObjective.Aggressive && (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider))
            {
                num34 *= 1.2f;
            }

            return (num34 < 0f) ? 0f : num34;
        }

        public override float CalculatePatrollingScoreForSettlement(Settlement targetSettlement, bool isFromPort, MobileParty mobileParty) => _model.CalculatePatrollingScoreForSettlement(targetSettlement, isFromPort, mobileParty);

        public override float CurrentObjectiveValue(MobileParty mobileParty) => _model.CurrentObjectiveValue(mobileParty);

        public override float GetPatrollingFactor(bool isNavalPatrolling) => _model.GetPatrollingFactor(isNavalPatrolling);

        public override float TravelingToAssignmentFactor => _model.TravelingToAssignmentFactor;

        public override float BesiegingFactor => _model.BesiegingFactor;

        public override float AssaultingTownFactor => _model.AssaultingTownFactor;

        public override float RaidingFactor => _model.RaidingFactor;

        public override float DefendingFactor => _model.DefendingFactor;
    }
}
