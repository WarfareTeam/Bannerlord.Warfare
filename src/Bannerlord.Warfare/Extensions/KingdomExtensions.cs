using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Warfare.Extensions
{
    internal static class KingdomExtensions
    {
        internal static void CreateArmy(this Kingdom kingdom, Hero leader, List<MobileParty> parties, bool disorganize = true)
        {
            Army army = new Army(kingdom, leader.PartyBelongedTo, Army.ArmyTypes.Patrolling)
            {
                AIBehavior = Army.AIBehaviorFlags.Gathering
            };
            army.GetType().GetField("_armyGatheringTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(army, Campaign.CurrentTime);
            if (army.LeaderParty != MobileParty.MainParty)
            {
                army.AiBehaviorObject = (IMapPoint)army.GetType().GetMethod("FindBestInitialGatheringSettlement", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(army, new object[] { leader.HomeSettlement });
                Settlement settlement = (Settlement)army.AiBehaviorObject;
                Vec2 centerPosition = (settlement.IsFortification ? settlement.GatePosition : settlement.Position2D);
                army.LeaderParty.GetType().GetMethod("SendPartyToReachablePointAroundPosition", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(army.LeaderParty, new object[] { centerPosition, 6f, 3f });
            }
            else
            {
                army.AiBehaviorObject = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsFortification || x.IsVillage);
            }
            GatherArmyAction.Apply(army.LeaderParty, (Settlement)army.AiBehaviorObject);
            kingdom.GetType().GetProperty("LastArmyCreationDay", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(kingdom, (int)CampaignTime.Now.ToDays);
            CampaignEventDispatcher.Instance.OnArmyCreated(army);
            if (leader == Hero.MainHero)
            {
                (Game.Current.GameStateManager.GameStates.Single((GameState S) => S is MapState) as MapState)?.OnArmyCreated(MobileParty.MainParty);
            }
            float influence = leader.Clan.Influence;
            foreach (MobileParty party in parties)
            {
                if (party != leader.PartyBelongedTo)
                {
                    party.Army = leader.PartyBelongedTo.Army;
                    CampaignEventDispatcher.Instance.OnPartyJoinedArmy(party);
                    SetPartyAiAction.GetActionForEscortingParty(party, leader.PartyBelongedTo);
                }
                party.GetType().GetField("_disorganizedUntilTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(party, disorganize ? CampaignTime.HoursFromNow(Campaign.Current.Models.PartyImpairmentModel.GetDisorganizedStateDuration(party)) : CampaignTime.Now);
                party.GetType().GetField("_isDisorganized", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(party, disorganize);
            }
            if (influence != leader.Clan.Influence)
            {
                leader.Clan.Influence = influence;
            }
        }
    }
}
