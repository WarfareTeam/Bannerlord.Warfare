using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using Warfare.Content.Contracts;
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
                IEnumerable<StanceLink> wars = FactionHelper.GetStances(kingdom).WhereQ(y => y.IsAtWar && y.Faction1 is Kingdom && y.Faction2 is Kingdom);
                int warCount = wars.CountQ();
                if (warCount < Settings.Current.MinimumWarsToHireMercenaries)
                {
                    continue;
                }
                foreach (Clan clan in kingdom.Clans.ToListQ())
                {
                    if (clan.IsEliminated || clan.IsMinorFaction || !clan.IsNoble || clan == Clan.PlayerClan)
                    {
                        continue;
                    }
                    IEnumerable<Clan> hireables = Clan.NonBanditFactions.WhereQ(x => !x.IsEliminated && x.IsMinorFaction && x != Clan.PlayerClan && (FindContract(x) == null || CanExtendContract(x, kingdom)) && clan.Leader.Gold >= x.GetMercenaryWage()).OrderByQ(x => x.GetMercenaryWage());
                    int hireableCount = hireables.CountQ();
                    if (hireableCount < 1)
                    {
                        continue;
                    }
                    float hireableMercenariesScore = hireableCount / 1200f;
                    float oppositionStrengthScore = wars.SelectQ(x => x.Faction1 == kingdom ? x.Faction2.CurrentTotalStrength - x.Faction1.CurrentTotalStrength : x.Faction1.CurrentTotalStrength - x.Faction2.CurrentTotalStrength).Sum() * 0.00004f;
                    float kingdomWarScore = warCount * 0.0025f + wars.Where(y => y.Faction2 == kingdom).CountQ() * 0.0225f;
                    float clanWealthScore = clan.Leader.Gold / 200_000_000f > 0.00001f ? clan.Leader.Gold / 200_000_000f : 0.00001f;
                    float clanHireMercenaryScore = hireableMercenariesScore + oppositionStrengthScore + kingdomWarScore + clanWealthScore;
                    Clan mercenary = hireables.ElementAt(MBRandom.RandomInt(hireableCount));
                    Contract contract = FindContract(mercenary);
                    if (contract != null)
                    {
                        float expirationScore = contract.Expiration.RemainingDaysFromNow / 4f;
                        clanHireMercenaryScore /= expirationScore / 4f > 1f ? expirationScore / 4f : 1f;
                    }
                    float RandomFloat = MBRandom.RandomFloat;
                    if (RandomFloat < clanHireMercenaryScore)
                    {
                        SignContract(mercenary, kingdom);
                        Hero armyLeader = (from x in clan.Heroes where x.PartyBelongedTo != null && x.PartyBelongedTo.Army != null orderby x.PartyBelongedTo.Army.EstimatedStrength descending select x).FirstOrDefault();
                        if (armyLeader != null)
                        {
                            foreach (WarPartyComponent party in mercenary.WarPartyComponents)
                            {
                                if (party.MobileParty == null || party.MobileParty.IsActive || party.MobileParty.MapEvent != null || party.MobileParty.SiegeEvent != null)
                                {
                                    continue;
                                }
                                party.MobileParty.Army = armyLeader.PartyBelongedTo.Army;
                                SetPartyAiAction.GetActionForEscortingParty(party.MobileParty, armyLeader.PartyBelongedTo.Army.LeaderParty, MobileParty.NavigationType.Default, false, false);
                            }
                        }
                        GiveGoldAction.ApplyBetweenCharacters(clan.Leader, mercenary.Leader, mercenary.GetMercenaryWage(), true);
                    }
                    if (Settings.Current.Logging)
                    {
                        SubModule.Log("Clan=" + clan.Name.ToString() + ", Mercenary=" + mercenary.Name.ToString() + ", RandomFloat=" + RandomFloat + ", clanHireMercenaryScore=" + clanHireMercenaryScore + ", hireableMercenariesScore=" + hireableMercenariesScore + ", oppositionStrengthScore=" + oppositionStrengthScore + ", kingdomWarScore=" + kingdomWarScore + ", clanWealthScore=" + clanWealthScore);
                    }
                }
            }
        }

        public Contract SignContract(Clan mercenary, Kingdom employer = null!) => _contractManager.SignContract(mercenary, employer);

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
