using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
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
            //Build initial mercenary clans & rosters on game start.
            //Has to be done after CampaignEvents.OnNewGameCreated and CampaignEvents.OnNewGameCreatedPartialFollowUpEvent
            //because the original method is done in the latter, and we need all WarPartyComponents initialized first.
            IEnumerable<CultureObject> cultures = from x in Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>() where x.StringId != "darshi" && x.StringId != "vakken" && x.StringId != "nord" select x;
            for (int i = 0; i < 6; i++)
            {
                CultureObject culture = cultures.ElementAt(i);
                string stringId = culture.StringId;
                CultureObject minorCulture = stringId == "aserai" ? Campaign.Current.ObjectManager.GetObject<CultureObject>("darshi") : stringId == "battania" ? Campaign.Current.ObjectManager.GetObject<CultureObject>("vakken") : stringId == "sturgia" ? Campaign.Current.ObjectManager.GetObject<CultureObject>("nord") : null!;
                IEnumerable<Clan> clans = from x in Clan.NonBanditFactions where (x.Culture == culture || x.Culture == minorCulture) && !x.IsEliminated && x.IsMinorFaction && x != Clan.PlayerClan select x;
                IList<CharacterObject> template = clans.GetRandomElementInefficiently().MinorFactionCharacterTemplates;
                for (int j = 0; j < clans.Count(); j++)
                {
                    Clan clan = clans.ElementAt(j);
                    foreach (Hero hero in clan.Heroes)
                    {
                        if (!hero.IsDead && hero != Hero.MainHero)
                        {
                            TextObject name = new TextObject(hero.Name.ToString().Split(' ').FirstOrDefault());
                            hero.SetName(name, name);
                        }
                        hero.Gold = 0;
                        GiveGoldAction.ApplyBetweenCharacters(null, hero, GetStartingGold());
                    }
                    clan.GetType().GetField("_banner", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(clan, Banner.CreateRandomClanBanner(MBRandom.RandomInt()));
                    clan.GetType().GetProperty("EncyclopediaText", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(clan, new TextObject());
                    if (minorCulture != null && clan.Culture == minorCulture)
                    {
                        clan.Culture = culture;
                    }
                    ChangeClanName(clan);
                    clan.ResetClanRenown();
                    clan.AddRenown(GetStartingRenown(clan));
                    RecruitTroops(clan, true);
                }
                for (int k = clans.Count(); k < 5; k++)
                {
                    Clan clan = Clan.CreateClan("WF_" + Clan.All.Count);
                    Settlement settlement = Settlement.All.GetRandomElementWithPredicate((Settlement x) => x.Culture == culture);
                    Vec2 centerPosition = MobilePartyHelper.FindReachablePointAroundPosition(settlement.GatePosition, 250f, 25f);
                    clan.InitializeClan(clan.Name, clan.Name, culture, Banner.CreateRandomClanBanner(MBRandom.RandomInt()), centerPosition);
                    clan.GetType().GetProperty("IsMinorFaction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(clan, true);
                    clan.GetType().GetField("_minorFactionCharacterTemplates", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(clan, template);
                    HeroSpawnCampaignBehavior behavior = Campaign.Current.GetCampaignBehavior<HeroSpawnCampaignBehavior>();
                    behavior.GetType().GetMethod("SpawnMinorFactionHeroes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Invoke(behavior, new object[] { clan, true });
                    behavior.GetType().GetMethod("CheckAndAssignClanLeader", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Invoke(behavior, new object[] { clan });
                    ChangeClanName(clan);
                    clan.UpdateHomeSettlement(null);
                    clan.AddRenown(GetStartingRenown(clan));
                    foreach (Hero hero in clan.Heroes)
                    {
                        GiveGoldAction.ApplyBetweenCharacters(null, hero, GetStartingGold());
                        clan.CreateNewMobileParty(hero).Ai.SetMoveModeHold();
                    }
                    RecruitTroops(clan, true);
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
            if (queue.IsEmpty() || !queue.Contains(clan))
            {
                return;
            }
            RecruitTroops(clan);
            queue.Remove(clan);
        }

        public void OnDailyTickClan(Clan clan)
        {
            if (!queue.IsEmpty() && queue.Contains(clan))
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
            string suffix = new string[] { "Band", "Companions", "Company", "Followers", "Wanderers" }.GetRandomElement();
            TextObject name = GameTexts.FindText("str_mercenary_name").SetTextVariable("LEADER", clan.Leader.Name).SetTextVariable("SUFFIX", suffix);
            clan.ChangeClanName(name, name);
        }

        public void RecruitTroops(Clan clan, bool newGame = false)
        {
            if (clan == null || clan.IsEliminated || !clan.IsMinorFaction || clan == Clan.PlayerClan)
            {
                return;
            }
            int limit = newGame ? clan.GetRosterLimit() - clan.GetRosterSize() : MathF.Min(clan.GetRosterLimit() - clan.GetRosterSize(), 10 + (int)Campaign.Current.CampaignStartTime.ElapsedSeasonsUntilNow);
            for (int i = 0; i < limit; i++)
            {
                IEnumerable<Hero> heroes = clan.WarPartyComponents.Where(x => x.MobileParty.MapEvent == null && x.MobileParty.SiegeEvent == null).OrderBy(x => x.Party.NumberOfAllMembers).Select(x => x.PartyOwner);
                if (heroes.Count() < 1)
                {
                    queue.Add(clan);
                    return;
                }
                Hero hero = heroes.First();
                if (newGame || MBRandom.RandomFloat < 0.6f) //(closestSettlement != null && MBRandom.RandomFloat < 0.6f + (closestSettlement.Town.Prosperity < 3000f ? 3f : closestSettlement.Town.Prosperity < 6000f ? 2f : 1f / 9f)))
                {
                    hero.PartyBelongedTo.AddElementToMercenaryRoster();
                }
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Empty
        }
    }
}