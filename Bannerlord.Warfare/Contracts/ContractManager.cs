﻿using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

using TaleWorlds.SaveSystem;

namespace Warfare.Contracts
{
    internal sealed class ContractManager
    {
        [SaveableField(1)]
        private List<Contract> _contracts;

        internal ContractManager() => _contracts ??= new();

        internal void CheckForExpirations()
        {
            foreach (Contract contract in _contracts.ToList())
            {
                if (CampaignTime.Now >= contract.Expiration)
                {
                    RemoveContract(contract.Mercenary);
                }
            }
        }

        public void SignContract(Clan mercenary, Kingdom employer)
        {
            Contract contract = FindContract(mercenary);
            if (contract != null)
            {
                contract.Expiration += CampaignTime.DaysFromNow(CampaignTime.DaysInSeason);
                return;
            }
            ChangeKingdomAction.ApplyByJoinFactionAsMercenary(mercenary, employer, 0, true);
            foreach (WarPartyComponent p in mercenary.WarPartyComponents)
            {
                if (p.MobileParty != null && p.MobileParty.CurrentSettlement == null)
                {
                    p.MobileParty.Ai.SetMoveModeHold();
                }
            }
            _contracts.Add(new Contract(employer, mercenary, CampaignTime.Now + CampaignTime.DaysFromNow(CampaignTime.DaysInSeason)));
        }

        public void RemoveContract(Clan mercenary)
        {
            Contract contract = FindContract(mercenary);
            if (contract == null)
            {
                return;
            }
            contract.Mercenary.Kingdom = null;
            CampaignEventDispatcher.Instance.OnClanChangedKingdom(contract.Mercenary, contract.Employer, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary, true);
            contract.Mercenary.EndMercenaryService(true);
            foreach (WarPartyComponent p in contract.Mercenary.WarPartyComponents)
            {
                if (p.MobileParty != null && p.MobileParty.CurrentSettlement == null)
                {
                    p.MobileParty.Ai.SetMoveModeHold();
                }
            }
            _contracts.Remove(contract);
        }
        public bool CanExtendContract(Clan mercenary, Kingdom employer)
        {
            Contract contract = FindContract(mercenary);
            return contract.Employer == employer && contract.Expiration.RemainingHoursFromNow <= CampaignTime.HoursInDay * CampaignTime.DaysInSeason;
        }

        public Contract FindContract(Clan mercenary)
        {
            foreach (Contract contract in _contracts)
            {
                if (contract.Mercenary == mercenary)
                {
                    return contract;
                }
            }
            return null!;
        }
    }
}
