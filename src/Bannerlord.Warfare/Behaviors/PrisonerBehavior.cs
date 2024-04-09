using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

using Warfare.Prisoners;

namespace Warfare.Behaviors
{
    /*
    internal sealed class PrisonerBehavior : CampaignBehaviorBase
    {
        internal PrisonerManager _prisonerManager;

        public static List<Prisoner>? _potentialPrisoners;

        public PrisonerBehavior()
        {
            _prisonerManager ??= new();
            _potentialPrisoners ??= new();
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnMainPartyPrisonerRecruitedEvent.AddNonSerializedListener(this, OnMainPartyPrisonerRecruitedEvent);
            CampaignEvents.MapEventStarted.AddNonSerializedListener(this, OnMapEventStarted);
            CampaignEvents.OnPrisonerDonatedToSettlementEvent.AddNonSerializedListener(this, OnPrisonerDonatedToSettlementEvent);
            CampaignEvents.OnPrisonerSoldEvent.AddNonSerializedListener(this, OnPrisonerSoldEvent);
            //CampaignEvents.OnPrisonerReleasedEvent.AddNonSerializedListener(this, OnPrisonerReleasedEvent);
            //AI prisoners don't escape or get released by captor???
        }

        private void OnMainPartyPrisonerRecruitedEvent(FlattenedTroopRoster roster)
        {
            RemovePrisoners(Hero.MainHero, roster);
        }

        private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
        {
            if (attackerParty != null && attackerParty.LeaderHero != null)
            {
                AddPotentialPrisoners(attackerParty.LeaderHero, attackerParty.MemberRoster);
                if (attackerParty.LeaderHero.PartyBelongedTo != null && attackerParty.LeaderHero.PartyBelongedTo.AttachedParties != null)
                {
                    foreach (MobileParty party in attackerParty.MobileParty.AttachedParties)
                    {
                        AddPotentialPrisoners(party.LeaderHero, party.MemberRoster);
                    }
                }
            }
            if (defenderParty != null && defenderParty.LeaderHero != null)
            {
                AddPotentialPrisoners(defenderParty.LeaderHero, defenderParty.MemberRoster);
                if (defenderParty.LeaderHero.PartyBelongedTo != null && defenderParty.LeaderHero.PartyBelongedTo.AttachedParties != null)
                {
                    foreach (MobileParty party in defenderParty.MobileParty.AttachedParties)
                    {
                        AddPotentialPrisoners(party.LeaderHero, party.MemberRoster);
                    }
                }
            }
        }

        private void OnPrisonerDonatedToSettlementEvent(MobileParty party, FlattenedTroopRoster roster, Settlement settlement)
        {
            if (party.LeaderHero != settlement.Owner)
            {
                int count = 0;
                IEnumerable<Prisoner> prisoners = FindPrisonersForCaptor(party.LeaderHero);
                if (prisoners != null && !prisoners.IsEmpty())
                {
                    foreach (FlattenedTroopRosterElement element in roster)
                    {
                        foreach (Prisoner prisoner in prisoners)
                        {
                            bool nextElement = false;
                            foreach (FlattenedTroopRosterElement prisonerElement in prisoner.Roster.ToFlattenedRoster())
                            {
                                if (element.Troop.StringId == prisonerElement.Troop.StringId)
                                {
                                    TroopRoster tempRoster = new TroopRoster(party.LeaderHero.PartyBelongedTo.Party);
                                    tempRoster.AddToCounts(element.Troop, 1);
                                    RemovePrisoner(prisoner, prisonerElement.Troop);
                                    AddPrisoner(prisoner.Owner, settlement.Owner, tempRoster);
                                    nextElement = true;
                                    count++;
                                    break;
                                }
                            }
                            if (nextElement)
                            {
                                break;
                            }
                        }
                    }
                }
                if (Settings.Current.Logging)
                {
                    SubModule.Log("Captor=" + party.LeaderHero.Name + ", newCaptor=" + settlement.Owner.Name + ", Count=" + count);
                }
            }
        }

        private void OnPrisonerSoldEvent(PartyBase sellerParty, PartyBase buyerParty, TroopRoster prisonerRoster)
        {
            RemovePrisoners(sellerParty.Owner, prisonerRoster.ToFlattenedRoster());
        }

        private void RemovePrisoners(Hero captor, FlattenedTroopRoster prisonerRoster)
        {
            IEnumerable<Prisoner> prisoners = FindPrisonersForCaptor(captor);
            if (prisoners != null && !prisoners.IsEmpty())
            {
                foreach (FlattenedTroopRosterElement element in prisonerRoster)
                {
                    foreach (Prisoner prisoner in prisoners)
                    {
                        bool nextElement = false;
                        foreach (FlattenedTroopRosterElement prisonerElement in prisoner.Roster.ToFlattenedRoster())
                        {
                            if (element.Troop.StringId == prisonerElement.Troop.StringId)
                            {
                                RemovePrisoner(prisoner, prisonerElement.Troop);
                                nextElement = true;
                                break;
                            }
                        }
                        if (nextElement)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void CheckPotentialPrisoners(TroopRoster prisonerRoster, PartyBase winnerParty)
        {
            foreach (TroopRosterElement element in prisonerRoster.GetTroopRoster())
            {
                foreach (Prisoner prisoner in _potentialPrisoners)
                {
                    int count = 0;
                    foreach (TroopRosterElement potentialPrisonerElement in prisoner.Roster.GetTroopRoster())
                    {
                        if (element.Character.StringId == potentialPrisonerElement.Character.StringId)
                        {
                            int toAdd = count + potentialPrisonerElement.Number > element.Number ? element.Number - potentialPrisonerElement.Number : potentialPrisonerElement.Number;
                            TroopRoster roster = new TroopRoster(prisoner.Owner.PartyBelongedTo.Party);
                            roster.AddToCounts(element.Character, toAdd);
                            AddPrisoner(prisoner.Owner, winnerParty.LeaderHero, roster);
                            count += toAdd;
                            break;
                        }
                    }
                    if (count >= element.Number)
                    {
                        break;
                    }
                }
            }
            _potentialPrisoners.Clear();
        }

        public void AddPotentialPrisoners(Hero owner, TroopRoster prisonersRoster) => _potentialPrisoners.Add(new Prisoner(owner, null!, prisonersRoster));

        public void RemovePotentialPrisoners(Prisoner potentialPrisoners) => _potentialPrisoners.Remove(potentialPrisoners);

        public Prisoner FindPotentialPrisoners(Hero hero)
        {
            foreach (Prisoner prisoner in _potentialPrisoners)
            {
                if (prisoner.Owner == hero)
                {
                    return prisoner;
                }
            }
            return null!;
        }

        public void CheckValid() => _prisonerManager.CheckValid();

        public void AddPrisoner(Hero owner, Hero captor, TroopRoster prisonersRoster) => _prisonerManager.AddPrisoner(owner, captor, prisonersRoster);

        public void RemovePrisoner(Prisoner prisoner, CharacterObject troop) => _prisonerManager.RemovePrisoner(prisoner, troop);

        public void ReleasePrisoners(Hero owner, Hero captor, FlattenedTroopRoster prisonersRoster) => _prisonerManager.ReleasePrisoners(owner, captor, prisonersRoster);

        public Prisoner FindPrisonerForCaptor(Hero captor) => _prisonerManager.FindPrisonerForCaptor(captor);

        public IEnumerable<Prisoner> FindPrisonersForCaptor(Hero captor) => _prisonerManager.FindPrisonersForCaptor(captor);

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_prisonerManager", ref _prisonerManager);
            if (dataStore.IsLoading)
            {
                _prisonerManager ??= new();
            }
        }
    }
    */
}
