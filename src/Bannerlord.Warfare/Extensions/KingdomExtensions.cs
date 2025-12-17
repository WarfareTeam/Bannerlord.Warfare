using Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Army army = new Army(kingdom, leader.PartyBelongedTo, Army.ArmyTypes.Patrolling);
            Settlement gatheringPoint = null;
            if (army.LeaderParty != MobileParty.MainParty)
            {
                army.GetType().GetMethod("FindBestGatheringSettlementAndMoveTheLeader", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(army, new object[] { leader.HomeSettlement });
            }
            else
            {

                Settlement settlement = SettlementHelper.FindNearestSettlementToMobileParty(leader.PartyBelongedTo, leader.PartyBelongedTo.NavigationCapability, (Settlement x) => x.IsFortification || x.IsVillage);
                if (settlement == null)
                {
                    CampaignVec2 point = MobileParty.MainParty.Position;
                    settlement = SettlementHelper.FindNearestSettlementToPoint(in point);
                }

                gatheringPoint = settlement;
            }
            GatherArmyAction.Apply(army.LeaderParty, gatheringPoint);
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
                    SetPartyAiAction.GetActionForEscortingParty(party, leader.PartyBelongedTo, MobileParty.NavigationType.Default, false, false);
                }
                if (party.AttachedTo == party.Army.LeaderParty)
                {
                    party.GetType().GetField("_disorganizedUntilTime", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(party, disorganize ? CampaignTime.HoursFromNow(Campaign.Current.Models.PartyImpairmentModel.GetDisorganizedStateDuration(party).ResultNumber) : CampaignTime.Now);
                    party.GetType().GetField("_isDisorganized", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(party, disorganize);
                }
            }
            if (influence != leader.Clan.Influence)
            {
                leader.Clan.Influence = influence;
            }
        }
    }
}
