using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace Warfare.Prisoners
{
    /*
    internal sealed class PrisonerManager
    {
        [SaveableField(1)]
        private List<Prisoner> _prisoners;

        internal PrisonerManager() => _prisoners ??= new();

        internal void CheckValid()
        {
            foreach (Prisoner prisoner in _prisoners.ToList())
            {
                Hero hero = prisoner.Owner;
                Hero captor = prisoner.Captor;
                TroopRoster roster = prisoner.Roster;
                if (hero.IsDead || !hero.IsPartyLeader || !hero.PartyBelongedTo.IsActive || captor.IsDead || !captor.IsPartyLeader || !captor.PartyBelongedTo.IsActive || roster.Count == 0)
                {
                    ClearPrisoner(hero, captor);
                }
            }
        }

        public void AddPrisoner(Hero owner, Hero captor, TroopRoster prisonerRoster)
        {
            Prisoner prisoner = FindPrisoner(owner, captor);
            if (prisoner != null)
            {
                prisoner.Roster.Add(prisonerRoster);
                return;
            }
            _prisoners.Add(new Prisoner(owner, captor, prisonerRoster));
        }

        public void RemovePrisoner(Prisoner prisoner, CharacterObject troop)
        {
            prisoner.Roster.RemoveTroop(troop);
        }

        public void ReleasePrisoners(Hero owner, Hero captor, FlattenedTroopRoster prisonerRoster)
        {
            Prisoner prisoner = FindPrisoner(owner, captor);
            if (prisoner != null)
            {
                int count = 0;
                foreach (FlattenedTroopRosterElement element in prisonerRoster)
                {
                    CharacterObject troop = element.Troop;
                    if (!prisoner.Roster.Contains(troop) || owner.IsDead || !owner.IsPartyLeader || !owner.PartyBelongedTo.IsActive || owner.PartyBelongedTo.MemberRoster.Count >= owner.PartyBelongedTo.LimitedPartySize)
                    {
                        continue;
                    }
                    RemovePrisoner(prisoner, troop);
                    owner.PartyBelongedTo.AddElementToMemberRoster(troop, 1);
                    count++;
                }
                if (prisoner.Roster.Count < 1)
                {
                    _prisoners.Remove(prisoner);
                }
                if (Settings.Current.Logging)
                {
                    SubModule.Log("Owner=" + owner.Name + ", Captor=" + captor.Name + ", Count=" + count);
                }
            }
        }

        public void ClearPrisoner(Hero owner, Hero captor)
        {
            Prisoner prisoner = FindPrisoner(owner, captor);
            if (prisoner == null)
            {
                return;
            }
            _prisoners.Remove(prisoner);
        }

        public Prisoner FindPrisoner(Hero owner, Hero captor)
        {
            foreach (Prisoner prisoner in _prisoners)
            {
                if (prisoner.Owner == owner && prisoner.Captor == captor)
                {
                    return prisoner;
                }
            }
            return null!;
        }

        public Prisoner FindPrisonerForCaptor(Hero captor)
        {
            IEnumerable<Prisoner> prisoners = FindPrisonersForCaptor(captor);
            if (!prisoners.IsEmpty())
            {
                if (prisoners.Count() == 1)
                {
                    return prisoners.FirstOrDefault();
                }
                return prisoners.GetRandomElementInefficiently();
            }
            return null!;
        }

        public IEnumerable<Prisoner> FindPrisonersForCaptor(Hero captor)
        {
            return from prisoner in _prisoners where captor == prisoner.Captor select prisoner;
        }
    }*/
}