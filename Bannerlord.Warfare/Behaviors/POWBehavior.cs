using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Warfare.Behaviors
{
    /*
    internal sealed class POWBehavior : CampaignBehaviorBase
    {

        internal PrisonerManager _powManager;

        public static IFaction? _lastDefeated;

        public POWBehavior()
        {
            _powManager ??= new();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, OnPlayerBattleEndEvent);
            CampaignEvents.Pris.AddNonSerializedListener(this, OnPlayerBattleEndEvent);
            //CampaignEvents.OnPrisonerTakenEvent.AddNonSerializedListener(this, OnPrisonerTaken);
            //CampaignEvents.OnPrisonerReleasedEvent.AddNonSerializedListener(this, OnPrisonerReleasedEvent);
            //CampaignEvents.OnMainPartyPrisonerRecruitedEvent.AddNonSerializedListener(this, OnMainPartyPrisonerRecruitedEvent);
            //CampaignEvents.OnPrisonerDonatedToSettlementEvent.AddNonSerializedListener(this, OnPrisonerDonatedToSettlementEvent);
            //CampaignEvents.OnPrisonerSoldEvent.AddNonSerializedListener(this, OnPrisonerSoldEvent);
        }

        public void OnTakePrisoners(FlattenedTroopRoster roster)
        {
            if (_lastDefeated != null)
            {
                _powManager.GetPrisonersOfWar(_lastDefeated);

            }
            //FlattenedTroopRoster roster2 = new FlattenedTroopRoster();
            //foreach (FlattenedTroopRosterElement element in roster)
            //{
            //    PrisonersOfWarBehavior.OnTakePrisoner(element);
            //}
        }

        private void OnPlayerBattleEndEvent(MapEvent e)
        {
            if ((from p in e.Winner.Parties where p.Party.MobileParty.IsMainParty select p).Count() > 0)
            {
                MBReadOnlyList<MapEventParty> opponents = e.Winner.MissionSide == BattleSideEnum.Attacker ? e.PartiesOnSide(BattleSideEnum.Defender) : e.PartiesOnSide(BattleSideEnum.Attacker);
                if (opponents.Count > 0)
                {
                    IEnumerable<IFaction> lastDefeated = from o in opponents where o.Party.MapFaction != null select o.Party.MapFaction;
                    if (lastDefeated.Count() > 0)
                    {
                        _lastDefeated = lastDefeated.First().MapFaction;
                        return;
                    }
                }
            }
            _lastDefeated = null;
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_prisonersOfWarManager", ref _powManager);

            if (dataStore.IsLoading)
            {
                _powManager ??= new();
            }
        }
    }
    */
}
