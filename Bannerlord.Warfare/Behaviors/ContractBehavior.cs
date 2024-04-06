using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.Core;

using Warfare.Contracts;
using Warfare.Extensions;

namespace Warfare.Behaviors
{
    internal sealed class ContractBehavior : CampaignBehaviorBase
    {
        internal ContractManager _contractManager;

        public ContractBehavior() => _contractManager ??= new();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        }

        public void OnDailyTick()
        {
            foreach (Kingdom kingdom in Kingdom.All)
            {   
                if (kingdom.IsEliminated || kingdom.Leader == Hero.MainHero)
                {
                    continue;
                }
                IEnumerable<StanceLink> wars = kingdom.Stances.Where(y => y.IsAtWar && y.Faction1 is Kingdom && y.Faction2 is Kingdom);
                int warCount = wars.Count();
                if (warCount < 1)
                {
                    continue;
                }
                foreach (Clan clan in kingdom.Clans.ToList())
                {
                    if (clan.IsEliminated || clan.IsMinorFaction || !clan.IsNoble || clan == Clan.PlayerClan)
                    {
                        continue;
                    }
                    IEnumerable<Clan> hireables = Clan.NonBanditFactions.Where(x => !x.IsEliminated && x.IsMinorFaction && x != Clan.PlayerClan && (FindContract(x) == null || CanExtendContract(x, kingdom)) && clan.Leader.Gold >= x.GetMercenaryWage()).OrderBy(x => x.GetMercenaryWage());
                    int hireableCount = hireables.Count();
                    if (hireableCount < 1)
                    {
                        continue;
                    }
                    float hireableMercenariesScore = hireableCount / 1200f;
                    float oppositionStrengthScore = wars.Select(x => x.Faction1 == kingdom ? x.Faction2.TotalStrength - x.Faction1.TotalStrength : x.Faction1.TotalStrength - x.Faction2.TotalStrength).Sum() * 0.00004f;
                    float kingdomWarScore = warCount * 0.0025f + wars.Where(y => y.Faction2 == kingdom).Count() * 0.0225f;
                    float clanWealthScore = clan.Leader.Gold / 200_000_000f > 0.00001f ? clan.Leader.Gold / 200_000_000f : 0.00001f;
                    float clanHireMercenaryScore = hireableMercenariesScore + oppositionStrengthScore + kingdomWarScore + clanWealthScore;
                    Clan mercenary = hireables.ElementAt(MBRandom.RandomInt(hireableCount));
                    Contract contract = FindContract(mercenary);
                    if (contract != null)
                    {
                        float expirationScore = contract.Expiration.RemainingDaysFromNow / 4f;
                        clanHireMercenaryScore /= expirationScore / 4f > 1f ? expirationScore / 4f : 1f;
                    }
                    if (MBRandom.RandomFloat < clanHireMercenaryScore)
                    {
                        SignContract(mercenary, kingdom);
                        if (clan.Leader != null && clan.Leader.PartyBelongedTo != null && clan.Leader.PartyBelongedTo.Army != null)
                        {
                            foreach (WarPartyComponent party in mercenary.WarPartyComponents)
                            {
                                if (party.MobileParty != null && party.MobileParty.IsActive && party.MobileParty.MapEvent == null && party.MobileParty.SiegeEvent == null)
                                {
                                    party.MobileParty.Army = clan.Leader.PartyBelongedTo.Army;
                                    SetPartyAiAction.GetActionForEscortingParty(party.MobileParty, clan.Leader.PartyBelongedTo.Army.LeaderParty);
                                }
                            }
                        }
                        GiveGoldAction.ApplyBetweenCharacters(clan.Leader, mercenary.Leader, mercenary.GetMercenaryWage(), true);
                    }
                }
            }
        }

        public void SignContract(Clan mercenary, Kingdom employer = null!) => _contractManager.SignContract(mercenary, employer);

        public bool CanExtendContract(Clan mercenary, Kingdom employer) => _contractManager.CanExtendContract(mercenary, employer);

        public Contract FindContract(Clan mercenary) => _contractManager.FindContract(mercenary);

        public void OnHourlyTick() => _contractManager.CheckForExpirations();

        public void RemoveContract(Clan mercenary) => _contractManager.RemoveContract(mercenary);

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_contractManager", ref _contractManager);
            if (dataStore.IsLoading)
            {
                _contractManager ??= new();
            }
        }
    }
}
