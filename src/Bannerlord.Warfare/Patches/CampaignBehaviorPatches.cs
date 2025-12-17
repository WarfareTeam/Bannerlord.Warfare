using Helpers;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.BarterBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.CommentBehaviors;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

using HarmonyLib;

using Warfare.Notifications;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(CommentOnChangeVillageStateBehavior), "OnVillageStateChanged")]
    public static class OnVillageStateChangedPatch
    {
        public static Village? _village;
        public static bool Prefix(Village village, Village.VillageStates oldState, Village.VillageStates newState, MobileParty raiderParty)
        {
            if (Settings.Current.EnableNotifications)
            {
                if (newState == Village.VillageStates.BeingRaided && ((Settings.Current.RaidNotificationScope.SelectedIndex == 1 && village.Settlement.OwnerClan.Kingdom == Clan.PlayerClan.Kingdom) || village.Settlement.OwnerClan == Clan.PlayerClan))
                {
                    int index = Settings.Current.RaidNotifications.SelectedIndex;
                    TextObject text = new VillageRaidedLogEntry(village, raiderParty).GetEncyclopediaText();
                    if (index == 1 || index == 3)
                    {
                        Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new VillageRaidedMapNotification(raiderParty, village, text));
                    }
                    if (index == 2 || index == 3)
                    {
                        _village = village;
                        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=cfwzuNa9}Village Being Raided").ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, GameTexts.FindText("str_show_on_map").ToString(), GameTexts.FindText("str_ok").ToString(), ExecuteShowOnMap, null), true);
                    }
                }
                return false;
            }
            return true;
        }
        private static void ExecuteShowOnMap()
        {
            if (_village != null)
            {
                MapScreen.Instance.FastMoveCameraToPosition(_village.Settlement.Position);
            }
        }
    }
    [HarmonyPatch(typeof(CommentOnDeclareWarBehavior), "OnWarDeclared")]
    public static class OnWarDeclaredPatch
    {
        public static bool Prefix(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
        {
            if (Settings.Current.EnableNotifications)
            {
                if (faction2 == Hero.MainHero.MapFaction || (faction1 != Hero.MainHero.MapFaction && detail != DeclareWarAction.DeclareWarDetail.CausedByKingdomDecision) || (Settings.Current.WarNotificationScope.SelectedIndex == 1 && faction2 != Hero.MainHero.MapFaction && faction1 != Hero.MainHero.MapFaction))
                {
                    int index = Settings.Current.WarNotifications.SelectedIndex;
                    TextObject text = new DeclareWarLogEntry(faction1, faction2).GetEncyclopediaText();
                    if (index == 1 || index == 3)
                    {
                        Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new WarMapNotification(faction1, faction2, text));
                    }
                    if (index == 2 || index == 3)
                    {
                        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=a5TpDQTY}War Declared").ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, GameTexts.FindText("str_ok").ToString(), null, null, null), true);
                    }
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(CommentOnMakePeaceBehavior), "OnMakePeace")]
    public static class OnMakePeacePatch
    {
        public static bool Prefix(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
        {
            if (Settings.Current.EnableNotifications)
            {
                if (faction2 == Hero.MainHero.MapFaction || (faction1 != Hero.MainHero.MapFaction && detail != MakePeaceAction.MakePeaceDetail.ByKingdomDecision) || (Settings.Current.PeaceNotificationScope.SelectedIndex == 1 && faction2 != Hero.MainHero.MapFaction && faction1 != Hero.MainHero.MapFaction))
                {
                    int index = Settings.Current.PeaceNotifications.SelectedIndex;
                    TextObject text = new MakePeaceLogEntry(faction1, faction2).GetEncyclopediaText();
                    if (index == 1 || index == 3)
                    {
                        Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new PeaceMapNotification(faction1, faction2, text));
                    }
                    if (index == 2 || index == 3)
                    {
                        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=KczmEYFF}Peace Declared").ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, GameTexts.FindText("str_ok").ToString(), null, null, null, null), true);
                    }
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(DefaultLogsCampaignBehavior), "OnArmyCreated")]
    public static class OnArmyCreatedPatch
    {
        public static Army? _army;
        public static bool Prefix(Army army)
        {
            if (Settings.Current.EnableNotifications)
            {
                KingdomState? state = Game.Current.GameStateManager.GameStates.FirstOrDefaultQ((GameState s) => s is KingdomState) as KingdomState;
                if ((state == null || !state.IsActive) && Settings.Current.EnableNotifications && army.LeaderParty.MapFaction == MobileParty.MainParty.MapFaction && army.LeaderParty != MobileParty.MainParty)
                {
                    int index = Settings.Current.ArmyCreatedNotifications.SelectedIndex;
                    TextObject text = new ArmyCreationLogEntry(army).GetEncyclopediaText();
                    if (index == 1 || index == 3)
                    {
                        Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ArmyCreationMapNotification(army, text));
                    }
                    if (index == 2 || index == 3)
                    {
                        _army = army;
                        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=SDRMX4x2}Army Created").ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_show_on_map").ToString(), GameTexts.FindText("str_ok").ToString(), ExecuteShowOnMap, null, null), true);
                    }
                }
                return false;
            }
            return true;
        }
        private static void ExecuteShowOnMap()
        {
            if (_army != null)
            {
                MapScreen.Instance.FastMoveCameraToPosition(_army.LeaderParty.Position);
            }
        }
    }
    [HarmonyPatch(typeof(DefaultLogsCampaignBehavior), "OnArmyDispersed")]
    public static class OnArmyDispersedPatch
    {
        public static bool Prefix(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
        {
            if (Settings.Current.EnableNotifications)
            {
                KingdomState? state = Game.Current.GameStateManager.GameStates.FirstOrDefaultQ((GameState s) => s is KingdomState) as KingdomState;
                if ((state == null || !state.IsActive) && army.ArmyOwner != null && army.Kingdom == Clan.PlayerClan.Kingdom && !army.Parties.Contains(MobileParty.MainParty) && !isPlayersArmy)
                {
                    int index = Settings.Current.ArmyDispersedNotifications.SelectedIndex;
                    TextObject text = GetEncyclopediaText(army.ArmyOwner, reason);
                    if (index == 1 || index == 3)
                    {
                        Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ArmyDispersionMapNotification(army, reason, text));
                    }
                    if (index == 2 || index == 3)
                    {
                        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=iQk7xM7N}Army Dispersed").ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: false, GameTexts.FindText("str_ok").ToString(), null, null, null, null), true);
                    }
                }
                return false;
            }
            return true;
        }
        private static TextObject GetEncyclopediaText(Hero hero, Army.ArmyDispersionReason reason)
        {
            TextObject textObject = reason switch
            {
                Army.ArmyDispersionReason.DismissalRequestedWithInfluence => new TextObject("{=2yRoDSFv}{LEADER_NAME} has disbanded their army near {SETTLEMENT} since dismissal is requested.", null),
                Army.ArmyDispersionReason.NotEnoughParty => new TextObject("{=jAZDcWDs}{LEADER_NAME} disbanded their army near {SETTLEMENT} since other parties have left the army.", null),
                Army.ArmyDispersionReason.KingdomChanged => new TextObject("{=qT7EdgVA}{LEADER_NAME} disbanded their army near {SETTLEMENT} since kingdom has been changed.", null),
                Army.ArmyDispersionReason.CohesionDepleted => new TextObject("{=VxIbrdlv}{LEADER_NAME} disbanded their army near {SETTLEMENT} since cohesion has been depleted.", null),
                Army.ArmyDispersionReason.ObjectiveFinished => new TextObject("{=g7A0NEoi}{LEADER_NAME} disbanded their army near {SETTLEMENT} since the objective is finished.", null),
                Army.ArmyDispersionReason.LeaderPartyRemoved => new TextObject("{=tT13MvdK}{LEADER_NAME} disbanded their army near {SETTLEMENT} since they have left the army.", null),
                Army.ArmyDispersionReason.CannotElectNewLeader => new TextObject("{=FpyapN2t}{LEADER_NAME} disbanded their army near {SETTLEMENT} since a new leader cannot be selected.", null),
                Army.ArmyDispersionReason.LeaderCannotArrivePointOnTime => new TextObject("{=YQ88av6Z}{LEADER_NAME} disbanded their army near {SETTLEMENT} since they couldn't arrive to the point on time.", null),
                Army.ArmyDispersionReason.ArmyLeaderIsDead => new TextObject("{=0XYI6M3o}{LEADER_NAME} had their army near {SETTLEMENT} disbanded since they are dead.", null),
                _ => new TextObject("{=tX7SnH06}{LEADER_NAME} disbanded their army near {SETTLEMENT}.", null),
            };
            if (hero.IsPrisoner)
            {
                textObject = new TextObject("{=f3acvV05}{LEADER_NAME} disbanded their army near {SETTLEMENT} since they were taken as a prisoner.", null);
            }
            textObject.SetTextVariable("LEADER_NAME", hero.Name);
            textObject.SetTextVariable("SETTLEMENT", HeroHelper.GetClosestSettlement(hero).Name);
            return textObject;
        }
    }
    [HarmonyPatch(typeof(DefaultLogsCampaignBehavior), "OnSiegeEventStarted")]
    public static class OnSiegeEventStartedPatch
    {
        private static Settlement? _settlement;
        public static bool Prefix(SiegeEvent siegeEvent)
        {
            if (Settings.Current.EnableNotifications)
            {
                if ((Settings.Current.SiegeNotificationScope.SelectedIndex == 1 && siegeEvent.BesiegedSettlement.OwnerClan.Kingdom == Clan.PlayerClan.Kingdom) || siegeEvent.BesiegedSettlement.OwnerClan == Clan.PlayerClan)
                {
                    int index = Settings.Current.SiegeNotifications.SelectedIndex;
                    TextObject text = new BesiegeSettlementLogEntry(siegeEvent.BesiegerCamp.LeaderParty, siegeEvent.BesiegedSettlement).GetEncyclopediaText();
                    if (index == 1 || index == 3)
                    {
                        Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new SettlementUnderSiegeMapNotification(siegeEvent, text));
                    }
                    if (index == 2 || index == 3)
                    {
                        _settlement = siegeEvent.BesiegedSettlement;
                        InformationManager.ShowInquiry(new InquiryData(new TextObject("{=1vl6Fx00}Settlement Besieged").ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_show_on_map").ToString(), GameTexts.FindText("str_ok").ToString(), ExecuteShowOnMap, null, null), true);
                    }
                }
                return false;
            }
            return true;
        }
        private static void ExecuteShowOnMap()
        {
            if (_settlement != null)
            {
                MapScreen.Instance.FastMoveCameraToPosition(_settlement.Position);
            }
        }
    }
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanJoinAsMercenary")]
    public static class ConsiderClanJoinAsMercenaryPatch
    {
        public static bool Prefix()
        {
            //Kingdoms decide when to hire mercenaries with this mod.
            return false;
        }
    }
    [HarmonyPatch(typeof(DiplomaticBartersBehavior), "ConsiderClanLeaveAsMercenary")]
    public static class ConsiderClanLeaveAsMercenaryPatch
    {
        public static bool Prefix()
        {
            //Mercenaries do not defect while under contract with this mod.
            return false;
        }
    }
    [HarmonyPatch(typeof(RecruitmentCampaignBehavior), "CheckRecruiting")]
    public static class RecruitmentCampaignBehaviorPatch
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            // Mercenary troop recruiting is handled in MercenaryBehavior with this mod.
            return Settings.Current.UseVanillaRecruitment || mobileParty == null || !mobileParty.IsActive || !mobileParty.IsLordParty || !mobileParty.ActualClan.IsMinorFaction || mobileParty.ActualClan == Clan.PlayerClan;
        }
    }
}
