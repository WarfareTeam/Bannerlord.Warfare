using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using Warfare.Extensions;

namespace Warfare.Behaviors
{
    internal sealed class MercenaryBehavior : CampaignBehaviorBase
    {

        List<Clan> queue = new List<Clan>();

        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, OnNewGameCreatedPartialFollowUpEnd);
            CampaignEvents.HourlyTickClanEvent.AddNonSerializedListener(this, OnHourlyTickClan);
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, OnDailyTickClan);
            CampaignEvents.BeforeHeroKilledEvent.AddNonSerializedListener(this, OnBeforeHeroKilled);
            CampaignEvents.OnClanLeaderChangedEvent.AddNonSerializedListener(this, OnClanLeaderChanged);
        }

        public void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
        {
            // Used in case a minor faction template couldn't be found
            // Prevents crashes on Europe 1100 (and possibly other total conversions) due to cultures without any clans / minor factions
            // Still needs further testing to figure out why some minor factions spawn without parties / troops on new game.
            IList<CharacterObject> backupTemplate = (from x in Clan.NonBanditFactions where !x.IsEliminated && x.IsMinorFaction && x != Clan.PlayerClan && !x.MinorFactionCharacterTemplates.IsEmpty() select x.MinorFactionCharacterTemplates).First();
            Settlement backupSettlement = (from x in Settlement.All where x.IsFortification select x).First();
            //Build initial mercenary clans & rosters on game start.
            //Has to be done after CampaignEvents.OnNewGameCreated and CampaignEvents.OnNewGameCreatedPartialFollowUpEvent
            //because the original method is done in the latter, and we need all WarPartyComponents initialized first.
            IEnumerable<CultureObject> cultures = from x in Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>() where x.StringId != "darshi" && x.StringId != "vakken" && x.StringId != "nord" select x;
            for (int i = 0; i < cultures.Count(); i++)
            {
                CultureObject culture = cultures.ElementAt(i);
                string stringId = culture.StringId;
                CultureObject minorCulture = stringId == "aserai" ? Campaign.Current.ObjectManager.GetObject<CultureObject>("darshi") : stringId == "battania" ? Campaign.Current.ObjectManager.GetObject<CultureObject>("vakken") : stringId == "sturgia" ? Campaign.Current.ObjectManager.GetObject<CultureObject>("nord") : null!;
                IEnumerable<Clan> clans = from x in Clan.NonBanditFactions where (x.Culture == culture || x.Culture == minorCulture) && !x.IsEliminated && x.IsMinorFaction && x != Clan.PlayerClan select x;
                for (int j = 0; j < clans.Count(); j++)
                {
                    Clan clan = clans.ElementAt(j);
                    foreach (Hero hero in clan.Heroes)
                    {
                        if (!Settings.Current.MaintainVanillaNames && !hero.IsDead && hero != Hero.MainHero)
                        {
                            TextObject name = new TextObject(hero.Name.ToString().Split(' ').FirstOrDefault());
                            hero.SetName(name, name);
                        }
                        if (Settings.Current.MaintainVanillaProperties)
                        {
                            hero.Gold = 0;
                            GiveGoldAction.ApplyBetweenCharacters(null, hero, GetStartingGold());
                        }
                    }
                    if (!Settings.Current.MaintainVanillaBanners)
                    {
                        clan.GetType().GetField("_banner", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(clan, Banner.CreateRandomClanBanner(MBRandom.RandomInt()));
                    }
                    if (!Settings.Current.MaintainVanillaNames)
                    {
                        clan.GetType().GetProperty("EncyclopediaText", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(clan, new TextObject(""));
                        ChangeClanName(clan);
                    }
                    if (Settings.Current.MaintainVanillaProperties)
                    {
                        if (minorCulture != null && clan.Culture == minorCulture)
                        {
                            clan.Culture = culture;
                        }
                        clan.ResetClanRenown();
                        clan.AddRenown(GetStartingRenown(clan));
                        if (!Settings.Current.UseVanillaRecruitment)
                        {
                            RecruitTroops(clan, true);
                        }
                    }
                }
                if (Settings.Current.SpawnAdditionalMercenaries)
                {
                    IList<CharacterObject> template = clans.IsEmpty() || clans.GetRandomElementInefficiently() == null || clans.GetRandomElementInefficiently().MinorFactionCharacterTemplates == null || clans.GetRandomElementInefficiently().MinorFactionCharacterTemplates.IsEmpty() ? backupTemplate : clans.GetRandomElementInefficiently().MinorFactionCharacterTemplates;
                    for (int k = clans.Count(); k < Settings.Current.MercenaryClansPerCulture; k++)
                    {
                        Clan clan = Clan.CreateClan("WF_" + Clan.All.Count);
                        IEnumerable<Settlement> settlements = from x in Settlement.All where x.Culture == culture && x.IsFortification select x;
                        Settlement settlement = settlements.IsEmpty() || settlements.FirstOrDefault() == null ? backupSettlement : settlements.FirstOrDefault();
                        CampaignVec2 centerPosition = NavigationHelper.FindReachablePointAroundPosition(settlement.GatePosition, MobileParty.NavigationType.Default, 250f, 25f);
                        clan.ChangeClanName(clan.Name, clan.Name);
                        clan.Culture = culture;
                        clan.Banner = Banner.CreateRandomClanBanner(MBRandom.RandomInt());
                        clan.GetType().GetProperty("IsMinorFaction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(clan, true);
                        clan.GetType().GetField("_minorFactionCharacterTemplates", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(clan, template);
                        HeroSpawnCampaignBehavior behavior = Campaign.Current.GetCampaignBehavior<HeroSpawnCampaignBehavior>();
                        behavior.GetType().GetMethod("SpawnMinorFactionHeroes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Invoke(behavior, new object[] { clan, true });
                        behavior.GetType().GetMethod("CheckAndAssignClanLeader", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Invoke(behavior, new object[] { clan });
                        ChangeClanName(clan);
                        clan.SetInitialHomeSettlement(settlement);
                        clan.AddRenown(GetStartingRenown(clan));
                        foreach (Hero hero in clan.Heroes)
                        {
                            GiveGoldAction.ApplyBetweenCharacters(null, hero, GetStartingGold());
                            MobileParty party = LordPartyComponent.CreateLordParty(hero.CharacterObject.StringId, hero, centerPosition, 3f, settlement, hero);
                            party.SetMoveModeHold();
                        }
                        if (!Settings.Current.UseVanillaRecruitment)
                        {
                            RecruitTroops(clan, true);
                        }
                    }

                }
            }
        }

        public int GetStartingGold()
        {
            return MBRandom.RandomFloat < 0.5f ? MBRandom.RandomInt(5000, 30000) : MBRandom.RandomInt(30000, 120000);
        }

        public int GetStartingRenown(Clan clan)
        {
            return MBRandom.RandomInt(clan.Heroes.Count() * 125, clan.Heroes.Count() * 800);
        }

        public void OnHourlyTickClan(Clan clan)
        {
            if (Settings.Current.UseVanillaRecruitment || queue.IsEmpty() || !queue.Contains(clan))
            {
                return;
            }
            RecruitTroops(clan);
            queue.Remove(clan);
        }

        public void OnDailyTickClan(Clan clan)
        {
            if (Settings.Current.UseVanillaRecruitment || (!queue.IsEmpty() && queue.Contains(clan)))
            {
                return;
            }
            RecruitTroops(clan);
        }

        private void OnBeforeHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detai, bool showNotification = true)
        {
            if (victim == null || victim.Clan == null || !victim.Clan.IsMinorFaction || victim.Clan == Clan.PlayerClan || victim.Clan.Heroes.IsEmpty() || !victim.Clan.GetHeirApparents().IsEmpty())
            {
                return;
            }
            ChangeClanLeaderAction.ApplyWithSelectedNewLeader(victim.Clan, victim.Clan.Heroes.Where(x => !x.IsChild && x != victim && x.IsAlive && x.IsLord).GetRandomElementInefficiently());
        }

        private void OnClanLeaderChanged(Hero oldLeader, Hero newLeader)
        {
            if (newLeader == null || newLeader.Clan == null || !newLeader.Clan.IsMinorFaction || newLeader.Clan == Clan.PlayerClan)
            {
                return;
            }
            ChangeClanName(newLeader.Clan);
        }

        public void ChangeClanName(Clan clan)
        {
            string suffix = new string[] { "{=dxkaEIqK}Band", "{=eVDDsEed}Companions", "{=BfSQxgPM}Company", "{=h6b2NEHU}Followers", "{=5E9aDuRC}Wanderers" }.GetRandomElement();
            TextObject name = new TextObject("{=eFauEP8r}{LEADER}'s {SUFFIX}").SetTextVariable("LEADER", clan.Leader.Name.ToString().Split(' ').FirstOrDefault()).SetTextVariable("SUFFIX", suffix);
            clan.ChangeClanName(name, name);
        }

        public void RecruitTroops(Clan clan, bool newGame = false)
        {
            IEnumerable<CharacterObject> mercenaryObjects = CharacterObject.FindAll(c => c != null && c.Occupation == Occupation.Mercenary);
            if (clan == null || clan.IsEliminated || !clan.IsMinorFaction || clan == Clan.PlayerClan)
            {
                return;
            }
            int limit = newGame ? clan.GetRosterLimit() - clan.GetRosterSize() : MathF.Min(clan.GetRosterLimit() - clan.GetRosterSize(), 10 + (int)Campaign.Current.Models.CampaignTimeModel.CampaignStartTime.ElapsedSeasonsUntilNow);
            for (int i = 0; i < limit; i++)
            {
                IEnumerable<Hero> heroes = clan.WarPartyComponents.Where(x => (newGame || (x.Leader != null && x.Leader.IsActive && x.Leader.IsPartyLeader && x.MobileParty != null)) && x.MobileParty.IsActive && x.MobileParty.MapEvent == null && x.MobileParty.SiegeEvent == null).OrderBy(x => x.Party.NumberOfAllMembers).Select(x => x.Leader);
                if (heroes.Count() < 1)
                {
                    queue.Add(clan);
                    return;
                }
                Hero hero = heroes.First();
                if (newGame || MBRandom.RandomFloat < 0.6f) //(closestSettlement != null && MBRandom.RandomFloat < 0.6f + (closestSettlement.Town.Prosperity < 3000f ? 3f : closestSettlement.Town.Prosperity < 6000f ? 2f : 1f / 9f)))
                {
                    hero.PartyBelongedTo.AddElementToMemberRoster(mercenaryObjects.GetRandomElementInefficiently(), 1);
                }
            }
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}