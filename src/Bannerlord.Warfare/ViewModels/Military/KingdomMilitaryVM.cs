using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Helpers;
using SandBox.GauntletUI;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

using Bannerlord.UIExtenderEx.Attributes;

using Warfare.Behaviors;
using Warfare.Content.Contracts;
using Warfare.Extensions;
using Warfare.GauntletUI;
using Warfare.ViewModels.ArmyManagement;
using TaleWorlds.LinQuick;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Warfare.ViewModels.Military
{
    public class KingdomMilitaryVM : KingdomCategoryVM
    {
        internal class MercenaryItemComparer : IComparer<KingdomMercenaryItemVM>
        {
            int IComparer<KingdomMercenaryItemVM>.Compare(KingdomMercenaryItemVM x, KingdomMercenaryItemVM y)
            {
                return x.Clan.GetMercenaryWage().CompareTo(y.Clan.GetMercenaryWage());
            }
        }
        private ContractBehavior _behavior = Campaign.Current.GetCampaignBehavior<ContractBehavior>();

        private GauntletKingdomScreen _gauntlet;

        private bool _showArmies;

        private bool _showMercenaries;

        private ClanCardSelectionPopupVM _cardSelectionPopup;

        private bool _isAcceptableMercenarySelected;

        private readonly KingdomInterface _kingdomInterface;

        private readonly IViewDataTracker _viewDataTracker;

        private Kingdom _kingdom;

        private MBBindingList<KingdomArmyItemVM> _armies;

        private MBBindingList<KingdomMercenaryItemVM> _mercenaries;

        private KingdomArmyItemVM _currentSelectedArmy;

        private KingdomMercenaryItemVM _currentSelectedMercenary;

        private string _noMercenarySelectedText;

        private string _militaryText;

        private string _armiesText;

        private string _mercenariesText;

        private string _partiesText;

        private string _disbandText;

        private string _manageText;

        private string _hireText;

        private string _extendText;

        private string _fireText;

        private string _disbandActionExplanationText;

        private string _manageActionExplanationText;

        private string _hireActionExplanationText;

        private string _extendActionExplanationText;

        private string _fireActionExplanationText;

        private HintViewModel _changeLeaderHint;

        private HintViewModel _splitArmyHint;

        private HintViewModel _disbandHint;

        private HintViewModel _manageArmyHint;

        private HintViewModel _hireHint;

        private HintViewModel _extendHint;

        private HintViewModel _fireHint;

        private bool _canChangeCurrentArmyLeader;

        private bool _canSplitCurrentArmy;

        private bool _canDisbandCurrentArmy;

        private bool _canManageCurrentArmy;

        private bool _canShowLocationOfCurrentArmy;

        private bool _canHireCurrentMercenary;

        private bool _canExtendCurrentMercenary;

        private bool _canFireCurrentMercenary;

        private bool _shouldExtendCurrentMercenary;

        private int _minimumArmyCost;

        private string _minimumArmyCostLabel;

        private int _totalArmyCost;

        private string _totalArmyCostLabel;

        private int _disbandCost;

        private int _hireCost;

        private string _hireCostLabel;

        private string _remainingContractTimeLabel;

        public KingdomMilitaryVM()
        {
            _gauntlet = ScreenManager.TopScreen as GauntletKingdomScreen;
            _cardSelectionPopup = new();
            _kingdomInterface = new();
            _kingdom = Clan.PlayerClan.Kingdom;
            _viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
            _armies = new();
            _mercenaries = new();
            MinimumArmyCost = new();
            TotalArmyCost = new();
            DisbandCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfDisbandingArmy();
            ChangeLeaderHint = new();
            SplitArmyHint = new();
            DisbandHint = new();
            ManageArmyHint = new();
            HireHint = new();
            ExtendHint = new();
            FireHint = new();
            IsAcceptableItemSelected = false;
            IsAcceptableMercenarySelected = false;
            RefreshArmyList();
            RefreshMercenaryList();
            RefreshValues();
            ExecuteShowArmies();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            MilitaryText = new TextObject("{=4T0zfjz0}Military").ToString();
            ArmiesText = GameTexts.FindText("str_armies").ToString();
            MercenariesText = new TextObject("{=L3vpqJKB}Mercenaries").ToString();
            PartiesText = GameTexts.FindText("str_parties").ToString();
            ManageText = GameTexts.FindText("str_manage").ToString();
            ManageActionExplanationText = new TextObject("{=zMGaTGyR}Manage this army").ToString();
            DisbandText = new TextObject("{=xXSFaGW8}Disband").ToString();
            DisbandActionExplanationText = GameTexts.FindText("str_kingdom_disband_army_explanation").ToString();
            HireText = new TextObject("{=lzKbVSTG}Hire").ToString();
            HireActionExplanationText = new TextObject("{=PKwBY8J1}Sign Contract").ToString();
            ExtendText = new TextObject("{=7Id19Dwx}Extend").ToString();
            ExtendActionExplanationText = new TextObject("{=NhIpemI2}Extend Contract").ToString();
            FireText = new TextObject("{=sVqbgt98}Fire").ToString();
            FireActionExplanationText = new TextObject("{=efD1qfi2}Terminate contract").ToString();
            CategoryNameText = new TextObject("{=4T0zfjz0}Military").ToString();
            NoItemSelectedText = GameTexts.FindText("str_kingdom_no_army_selected").ToString();
            NoMercenarySelectedText = new TextObject("{=JknjvtkX}No Mercenary Selected").ToString();
            Armies.ApplyActionOnAllItems(delegate (KingdomArmyItemVM x)
            {
                x.RefreshValues();
            });
            CurrentSelectedArmy?.RefreshValues();
            Mercenaries.ApplyActionOnAllItems(delegate (KingdomMercenaryItemVM x)
            {
                x.RefreshValues();
            });
            CurrentSelectedMercenary?.RefreshValues();
            CardSelectionPopup.RefreshValues();
        }

        public void OnFrameTick()
        {
            if (_kingdomInterface.ViewModel != null)
            {
                if (_kingdomInterface.ViewModel is KingdomArmyManagementVM)
                {
                    (_kingdomInterface.ViewModel as KingdomArmyManagementVM).OnFrameTick(_kingdomInterface.Layer);
                }
                else
                {
                    (_kingdomInterface.ViewModel as SplitArmyVM).OnFrameTick(_kingdomInterface.Layer);
                }
            }
        }

        public void RefreshArmyList()
        {
            NotificationCount = _viewDataTracker.NumOfKingdomArmyNotifications;
            if (_kingdom != null)
            {
                Armies.Clear();
                foreach (Army army in _kingdom.Armies)
                {
                    Armies.Add(new KingdomArmyItemVM(army, OnArmySelection));
                }
            }
            if (Armies.Count == 0 && CurrentSelectedArmy != null)
            {
                OnArmySelection(null);
            }
            else if (Armies.Count > 0)
            {
                OnArmySelection(Armies[0]);
                CurrentSelectedArmy.IsSelected = true;
            }
        }

        public void RefreshMercenaryList()
        {
            Mercenaries.Clear();
            foreach (Clan clan in Clan.NonBanditFactions)
            {
                if (!clan.IsEliminated && clan.IsMinorFaction && clan != Clan.PlayerClan)
                {
                    Mercenaries.Add(new KingdomMercenaryItemVM(clan, OnMercenarySelection));
                }
            }
            Mercenaries.Sort(new MercenaryItemComparer());
            if (Mercenaries.Count == 0 && CurrentSelectedMercenary != null)
            {
                OnMercenarySelection(null);
            }
            else if (Mercenaries.Count > 0)
            {
                OnMercenarySelection(Mercenaries[0]);
                CurrentSelectedMercenary.IsSelected = true;
            }
        }

        public void SelectArmy(Army army)
        {
            foreach (KingdomArmyItemVM army2 in Armies)
            {
                if (army2.Army == army)
                {
                    OnArmySelection(army2);
                    break;
                }
            }
        }

        internal void OnArmySelection(KingdomArmyItemVM item)
        {
            if (CurrentSelectedArmy != item)
            {
                RefreshCurrentArmyVisuals(item);
                CurrentSelectedArmy = item;
                IsAcceptableItemSelected = item != null;
            }
        }

        private void OnMercenarySelection(KingdomMercenaryItemVM item)
        {
            if (CurrentSelectedMercenary != item)
            {
                RefreshCurrentMercenaryVisuals(item);
                CurrentSelectedMercenary = item;
                IsAcceptableMercenarySelected = item != null;
            }
        }

        private void RefreshCurrentArmyVisuals(KingdomArmyItemVM item)
        {
            if (item != null)
            {
                if (CurrentSelectedArmy != null)
                {
                    CurrentSelectedArmy.IsSelected = false;
                }
                CurrentSelectedArmy = item;
                NotificationCount = _viewDataTracker.NumOfKingdomArmyNotifications;
                IEnumerable<int> memberPartyInfluenceCosts = from p in CurrentSelectedArmy.Army.Parties where !p.IsMainParty select Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, p);
                MinimumArmyCost = memberPartyInfluenceCosts.OrderBy(x => x).FirstOrDefault();
                TotalArmyCost = memberPartyInfluenceCosts.Sum();
                DisbandCost = 0;
                if (_kingdom.RulingClan != Clan.PlayerClan && CurrentSelectedArmy.Army.LeaderParty != MobileParty.MainParty)
                {
                    MinimumArmyCost *= 2;
                    TotalArmyCost *= 2;
                    DisbandCost = Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfDisbandingArmy();
                }
                MinimumArmyCostLabel = MinimumArmyCost.ToString();
                TotalArmyCostLabel = TotalArmyCost.ToString();
                CanChangeCurrentArmyLeader = GetCanChangeCurrentArmyLeaderWithReason(out var disabledReason);
                ChangeLeaderHint.HintText = disabledReason;
                CanSplitCurrentArmy = GetCanSplitCurrentArmyWithReason(out disabledReason);
                SplitArmyHint.HintText = disabledReason;
                CanDisbandCurrentArmy = GetCanDisbandCurrentArmyWithReason(out disabledReason);
                DisbandHint.HintText = disabledReason;
                if (CurrentSelectedArmy != null)
                {
                    CanShowLocationOfCurrentArmy = CurrentSelectedArmy.Army.AiBehaviorObject is Settlement || CurrentSelectedArmy.Army.AiBehaviorObject is MobileParty;
                    CanManageCurrentArmy = GetCanManageCurrentArmyWithReason(out disabledReason);
                    ManageArmyHint.HintText = disabledReason;
                }
            }
        }

        private bool GetCanChangeCurrentArmyLeaderWithReason(out TextObject disabledReason)
        {
            if (!GetArmyActionIsEnabledWithReason(out var disabledReason2))
            {
                disabledReason = disabledReason2;
                return false;
            }

            if (Clan.PlayerClan.Influence < TotalArmyCost)
            {
                disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence");
                return false;
            }

            if (CurrentSelectedArmy.Parties.Count < 2)
            {
                disabledReason = new TextObject("{=vZrxepWp}Army does not have enough member parties.");
                return false;
            }
            disabledReason = new TextObject("{=uhxCnG4n}Change Army Leader");
            return true;
        }

        private bool GetCanSplitCurrentArmyWithReason(out TextObject disabledReason)
        {
            if (!GetArmyActionIsEnabledWithReason(out var disabledReason2))
            {
                disabledReason = disabledReason2;
                return false;
            }

            if (Clan.PlayerClan.Influence < MinimumArmyCost)
            {
                disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence");
                return false;
            }

            if (CurrentSelectedArmy.Parties.Count < 4)
            {
                disabledReason = new TextObject("{=vZrxepWp}Army does not have enough member parties.");
                return false;
            }

            disabledReason = new TextObject("{=8MvD4u4J}Split Army");
            return true;
        }

        private bool GetCanDisbandCurrentArmyWithReason(out TextObject disabledReason)
        {
            if (!GetArmyActionIsEnabledWithReason(out var disabledReason2))
            {
                disabledReason = disabledReason2;
                return false;
            }

            if (Clan.PlayerClan.Influence < DisbandCost)
            {
                disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence");
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private bool GetCanManageCurrentArmyWithReason(out TextObject disabledReason)
        {
            if (!GetArmyActionIsEnabledWithReason(out var disabledReason2))
            {
                disabledReason = disabledReason2;
                return false;
            }

            if (!CurrentSelectedArmy.Army.Parties.Contains(MobileParty.MainParty) && Hero.MainHero != CurrentSelectedArmy.Army.Kingdom.Leader)
            {
                disabledReason = new TextObject("{=kBGlNylO}You cannot manage an army unless you are the faction leader or an army member.");
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private bool GetArmyActionIsEnabledWithReason(out TextObject disabledReason)
        {
            Hero hero = CurrentSelectedArmy.Army.ArmyOwner;
            if (hero.IsPrisoner)
            {
                disabledReason = GameTexts.FindText("str_action_disabled_reason_prisoner");
                return false;
            }

            if (Clan.PlayerClan.IsUnderMercenaryService)
            {
                disabledReason = new TextObject("{=YS8yB9oA}You cannot manage armies as a mercenary.");
                return false;
            }

            if (GameStateManager.Current?.GameStates.Any((x) => x.IsMission) ?? false)
            {
                disabledReason = new TextObject("{=FdzsOvDq}This action is disabled while in a mission.");
                return false;
            }

            if (hero.PartyBelongedTo.SiegeEvent != null)
            {
                disabledReason = new TextObject("{=DvO4aGre}You can not perform this action while the army is in a siege.");
                return false;
            }

            MapEvent mapEvent = hero.PartyBelongedTo.MapEvent;
            if (mapEvent != null)
            {
                if (mapEvent.MapEventSettlement == null)
                {
                    disabledReason = GameTexts.FindText("str_action_disabled_reason_encounter");
                    return false;
                }

                Village village = mapEvent.MapEventSettlement.Village;
                if (village != null && village.VillageState == Village.VillageStates.BeingRaided && (mapEvent?.IsRaid ?? false))
                {
                    disabledReason = GameTexts.FindText("str_action_disabled_reason_raid");
                    return false;
                }

                if (mapEvent.MapEventSettlement.IsUnderSiege)
                {
                    disabledReason = new TextObject("{=DvO4aGre}You can not perform this action while the army is in a siege.");
                    return false;
                }
                disabledReason = new TextObject("{=aXPDpkAR}You can not perform this action while the army is in a map event.");
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private void RefreshCurrentMercenaryVisuals(KingdomMercenaryItemVM item)
        {
            if (item != null)
            {
                if (CurrentSelectedMercenary != null)
                {
                    CurrentSelectedMercenary.IsSelected = false;
                }
                CurrentSelectedMercenary = item;
                HireCost = item.HireCost;
                CanHireCurrentMercenary = GetCanHireCurrentMercenaryWithReason(out var disabledReason);
                HireHint.HintText = disabledReason;
                GameTexts.SetVariable("LEFT", "{=J1G2EXsn}Contract Cost");
                GameTexts.SetVariable("RIGHT", HireCost);
                HireCostLabel = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon").ToString();
                TextObject remainingContractTime = TextObject.Empty;
                if (item.IsHired)
                {
                    Contract contract = _behavior.FindContract(item.Clan);
                    if (contract == null)
                    {
                        contract = _behavior.SignContract(item.Clan);
                    }
                    remainingContractTime = new TextObject("{=!}{YEARS} {SEASONS} {DAYS} {HOURS}");
                    CampaignTime expiration = contract.Expiration;
                    int years = (int)(expiration - CampaignTime.Now).ToYears;
                    int seasons = (int)(expiration - CampaignTime.Now).ToSeasons - (CampaignTime.SeasonsInYear * years);
                    int days = (int)(expiration - CampaignTime.Now).ToDays - (CampaignTime.DaysInYear * years) - (CampaignTime.DaysInSeason * seasons);
                    int hours = (int)(expiration - CampaignTime.Now).ToHours - (CampaignTime.HoursInDay * CampaignTime.DaysInYear * years) - (CampaignTime.HoursInDay * CampaignTime.DaysInSeason * seasons) - (CampaignTime.HoursInDay * days);
                    if (years > 0)
                    {
                        GameTexts.SetVariable("YEAR_IS_PLURAL", (years > 1) ? 1 : 0);
                        GameTexts.SetVariable("YEAR", years);
                        string yearsLabel = GameTexts.FindText("str_YEAR_years").ToString();
                        if (seasons > 0 || days > 0 || hours > 0)
                        {
                            yearsLabel += ", ";
                        }
                        remainingContractTime.SetTextVariable("YEARS", yearsLabel);
                    }
                    if (seasons > 0)
                    {
                        GameTexts.SetVariable("SEASON_IS_PLURAL", (seasons > 1) ? 1 : 0);
                        GameTexts.SetVariable("SEASON", seasons);
                        string seasonsLabel = GameTexts.FindText("str_SEASON_seasons").ToString();
                        if (days > 0 || hours > 0)
                        {
                            seasonsLabel += ", ";
                        }
                        remainingContractTime.SetTextVariable("SEASONS", seasonsLabel);
                    }
                    if (days > 0)
                    {
                        GameTexts.SetVariable("DAY_IS_PLURAL", (days > 1) ? 1 : 0);
                        GameTexts.SetVariable("DAY", days);
                        string daysLabel = GameTexts.FindText("str_DAY_days").ToString();
                        if (hours > 0)
                        {
                            daysLabel += ", ";
                        }
                        remainingContractTime.SetTextVariable("DAYS", daysLabel);
                    }
                    if (hours > 0)
                    {
                        string hoursLabel = new TextObject("{=xg0izQ4X}{HOUR} {?HOUR_IS_PLURAL}hours{?}hour{\\?}").SetTextVariable("HOUR", hours).SetTextVariable("HOUR_IS_PLURAL", hours > 1 ? 1 : 0).ToString();
                        remainingContractTime.SetTextVariable("HOURS", hoursLabel); 
                    }
                }
                GameTexts.SetVariable("LEFT", "{=pc60DYCO}Remaining Contract Time");
                GameTexts.SetVariable("RIGHT", remainingContractTime);
                RemainingContractTimeLabel = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon").ToString();
                CanExtendCurrentMercenary = GetCanExtendCurrentMercenaryWithReason(out disabledReason);
                ExtendHint.HintText = disabledReason;
                ShouldExtendCurrentMercenary = item.Clan.IsUnderMercenaryService && item.Clan.Kingdom == Clan.PlayerClan.Kingdom;
                CanFireCurrentMercenary = GetCanFireCurrentMercenaryWithReason(out disabledReason);
                FireHint.HintText = disabledReason;
            }
        }

        private bool GetCanHireCurrentMercenaryWithReason(out TextObject disabledReason)
        {

            if (CurrentSelectedMercenary.Clan.IsUnderMercenaryService)
            {
                disabledReason = new TextObject("{=lFWDKhaG}Mercenaries are already under contract.");
                return false;
            }

            if (Hero.MainHero.Gold < HireCost)
            {
                disabledReason = GameTexts.FindText("str_decision_not_enough_gold");
                return false;
            }

            if (!GetMercenaryActionIsEnabledWithReason(out disabledReason))
            {
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private bool GetCanExtendCurrentMercenaryWithReason(out TextObject disabledReason)
        {

            if (!CurrentSelectedMercenary.Clan.IsUnderMercenaryService || CurrentSelectedMercenary.Clan.Kingdom != Clan.PlayerClan.Kingdom)
            {
                disabledReason = new TextObject("{=ciBBsXak}Mercenaries are contracted by another kingdom.");
                return false;
            }

            if (Hero.MainHero.Gold < HireCost)
            {
                disabledReason = GameTexts.FindText("str_decision_not_enough_gold");
                return false;
            }

            if (!_behavior.CanExtendContract(CurrentSelectedMercenary.Clan, _kingdom))
            {
                int length = Settings.Current.MercenaryContractType.SelectedIndex;
                disabledReason = new TextObject("{=t30VD50U}Can only extend contracts with less than one {LENGTH} remaining.");
                if (length == 0)
                {
                    disabledReason.SetTextVariable("LENGTH", "{=VM3rkJqn}week");
                    return false;
                }
                if (length == 1)
                {
                    disabledReason.SetTextVariable("LENGTH", "{=vwl3CMHE}season");
                    return false;
                }
                disabledReason.SetTextVariable("LENGTH", "{=HB9ZKpAW}year");
                return false;
            }

            if (CurrentSelectedMercenary.Clan.WarPartyComponents.Find((w) => w.MobileParty.MapEvent != null) != null)
            {
                disabledReason = new TextObject("{=3rl5kKcO}You cannot hire mercenaries while they're in a map event.");
                return false;
            }

            if (!GetMercenaryActionIsEnabledWithReason(out disabledReason))
            {
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private bool GetCanFireCurrentMercenaryWithReason(out TextObject disabledReason)
        {

            Contract contract = _behavior.FindContract(CurrentSelectedMercenary.Clan);
            if (contract == null)
            {
                disabledReason = new TextObject("{=mfKEPzhj}Mercenaries are not under contract.");
                return false;
            }

            if (contract.Employer != _kingdom)
            {
                disabledReason = new TextObject("{=ciBBsXak}Mercenaries are contracted by another kingdom.");
                return false;
            }

            if (!Hero.MainHero.IsFactionLeader)
            {
                disabledReason = new TextObject("{=pts0L1WR}Only rulers can terminate mercenary contracts.");
                return false;
            }
            if (Hero.MainHero.IsPrisoner)
            {
                disabledReason = GameTexts.FindText("str_action_disabled_reason_prisoner");
                return false;
            }

            if (PlayerEncounter.Current != null)
            {
                if (PlayerEncounter.EncounterSettlement == null)
                {
                    disabledReason = GameTexts.FindText("str_action_disabled_reason_encounter");
                    return false;
                }

                Village village = PlayerEncounter.EncounterSettlement.Village;
                if (village != null && village.VillageState == Village.VillageStates.BeingRaided && MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.IsRaid)
                {
                    disabledReason = GameTexts.FindText("str_action_disabled_reason_raid");
                    return false;
                }
            }

            if (!GetMercenaryActionIsEnabledWithReason(out disabledReason))
            {
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private bool GetMercenaryActionIsEnabledWithReason(out TextObject disabledReason)
        {
            Clan clan = CurrentSelectedMercenary.Clan;
            if (clan == Clan.PlayerClan)
            {
                disabledReason = new TextObject("{=fOPN5GCF}You cannot manage your own contract.");
                return false;
            }

            if (Clan.PlayerClan.IsUnderMercenaryService)
            {
                disabledReason = new TextObject("{=YS8yB9oA}You cannot manage armies as a mercenary.");
                return false;
            }

            IEnumerable<Hero> partyLeaders = clan.Lords.Where((h) => h.IsPartyLeader);
            if (!partyLeaders.Where((h) => h.PartyBelongedTo.MapEvent != null).IsEmpty())
            {
                disabledReason = new TextObject("{=nDwqp7GZ}You cannot manage contracts while a target clan party is in a map event.");
                return false;
            }

            if (!partyLeaders.Where((h) => h.IsPrisoner).IsEmpty())
            {
                disabledReason = GameTexts.FindText("str_action_disabled_reason_prisoner");
                return false;
            }

            if (GameStateManager.Current?.GameStates.Any((x) => x.IsMission) ?? false)
            {
                disabledReason = new TextObject("{=FdzsOvDq}This action is disabled while in a mission.");
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        [DataSourceMethod]
        public void ExecuteShowArmies()
        {
            ShowArmies = true;
            ShowMercenaries = false;
        }

        [DataSourceMethod]
        public void ExecuteShowMercenaries()
        {
            ShowArmies = false;
            ShowMercenaries = true;
        }

        [DataSourceMethod]
        public void ExecuteHireCurrentMercenary()
        {
            if (Hero.MainHero.Gold >= HireCost)
            {
                TextObject titleText = new TextObject("{=PKwBY8J1}Sign Contract");
                TextObject text = new TextObject("{=zrhr4rDA}Are you sure you want to {CONTRACT_TYPE} contract with this mercenary? This will result in a non-refundable up-front cost.").SetTextVariable("CONTRACT_TYPE", "{=AwmsTVoc}sign");
                if (ShouldExtendCurrentMercenary)
                {
                    titleText = new TextObject("{=NhIpemI2}Extend Contract");
                    text.SetTextVariable("CONTRACT_TYPE", "{=NgtpnwlS}extend");
                }
                InformationManager.ShowInquiry(new InquiryData(titleText.ToString(), text.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), HireCurrentMercenary, null));
            }
        }

        private void HireCurrentMercenary()
        {
            if (Hero.MainHero.Gold >= HireCost)
            {
                ChangeRelationAction.ApplyPlayerRelation(CurrentSelectedMercenary.Clan.Leader, 10, true, true);
                GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, CurrentSelectedMercenary.Clan.Leader, HireCost, true);
                _behavior.SignContract(CurrentSelectedMercenary.Clan, _kingdom);
                RefreshMercenaryList();
            }
        }

        [DataSourceMethod]
        public void ExecuteFireCurrentMercenary()
        {
            if (Hero.MainHero.IsFactionLeader)
            {
                InformationManager.ShowInquiry(new InquiryData(new TextObject("{=efD1qfi2}Terminate contract").ToString(), new TextObject("{=eZlBbp5f}Are you sure you want to terminate the contract with this mercenary? You will not receive a refund.").ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), FireCurrentMercenary, null));
            }
        }

        private void FireCurrentMercenary()
        {
            if (Hero.MainHero.IsFactionLeader)
            {
                ChangeRelationAction.ApplyPlayerRelation(CurrentSelectedMercenary.Clan.Leader, -10, true, true);
                _behavior.RemoveContract(CurrentSelectedMercenary.Clan);
                RefreshMercenaryList();
            }
        }

        [DataSourceMethod]
        public void ExecuteChangeLeader()
        {
            if (CurrentSelectedArmy != null)
            {
                string inquiryText = new TextObject("{=TOC7RWdV}Are you sure you want to change this armies leader?").ToString();
                if (CurrentSelectedArmy.Army.LeaderParty == MobileParty.MainParty)
                {
                    inquiryText += new TextObject("{=wPSl2v80}{newline}{newline}WARNING: You will not be added to the new army.").ToString();
                }
                InformationManager.ShowInquiry(new InquiryData(new TextObject("{=uhxCnG4n}Change Army Leader").ToString(), inquiryText, isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), OpenChangeLeaderPopup, null));
            }
        }

        public void OpenChangeLeaderPopup()
        {
            Action<ClanCardSelectionInfo> openPopup = new Action<ClanCardSelectionInfo>(_cardSelectionPopup.Open);
            ClanCardSelectionInfo obj = new ClanCardSelectionInfo(new TextObject("{=uhxCnG4n}Change Army Leader"), GetArmyLeaderCandidates(), OnChangeLeaderOver, isMultiSelection: false);
            openPopup.Invoke(obj);
        }

        [DataSourceMethod]
        public void ExecuteSplitArmy()
        {
            if (CurrentSelectedArmy != null)
            {
                string inquiryText = new TextObject("{=tyZxyL8d}Are you sure you want to split this army? It will cost you between {MIN_COST} and {MAX_COST} influence.").SetTextVariable("MIN_COST", MinimumArmyCost).SetTextVariable("MAX_COST", TotalArmyCost).ToString();
                if (CurrentSelectedArmy.Army.LeaderParty == MobileParty.MainParty)
                {
                    inquiryText += new TextObject("{=n7wMx7op}{newline}{newline}WARNING: Your army will lose the selected leader if they're a member and any member parties you select. You will remain the leader of the old army.").ToString();
                }
                InformationManager.ShowInquiry(new InquiryData(new TextObject("{=8MvD4u4J}Split Army").ToString(), inquiryText, isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), OpenSplitArmyPopup, null));
            }
        }

        public void OpenSplitArmyPopup()
        {
            if (CurrentSelectedArmy != null)
            {
                Action<ClanCardSelectionInfo> openPopup = new Action<ClanCardSelectionInfo>(_cardSelectionPopup.Open);
                ClanCardSelectionInfo obj = new ClanCardSelectionInfo(new TextObject("{=8MvD4u4J}Split Army"), GetArmyLeaderCandidates(), OnSplitArmyLeaderSelectOver, isMultiSelection: false);
                openPopup.Invoke(obj);
            }
        }

        private IEnumerable<ClanCardSelectionItemInfo> GetArmyLeaderCandidates()
        {
            foreach (Hero item in Clan.PlayerClan.Kingdom.Heroes.Where((h) => h.IsPartyLeader && (h.IsActive || h.IsReleased || h.IsFugitive || h.IsTraveling) && !h.IsChild && h.CanLeadParty() && h != CurrentSelectedArmy.Army.LeaderParty.LeaderHero && !h.Clan.IsUnderMercenaryService))
            {
                yield return new ClanCardSelectionItemInfo(item, item.Name, new ImageIdentifier(CampaignUIHelper.GetCharacterCode(item.CharacterObject)), CardSelectionItemSpriteType.None, null, null, GetArmyLeaderCandidateProperties(item), !IsFactionMemberAvailableForArmyLeaderChange(item, CurrentSelectedArmy.Army, out TextObject explanation), explanation, null);
            }
        }

        private IEnumerable<ClanCardSelectionItemPropertyInfo> GetArmyLeaderCandidateProperties(Hero hero)
        {
            TextObject skillsText = TextObject.Empty;
            foreach (SkillObject skill in new List<SkillObject> { DefaultSkills.Engineering, DefaultSkills.Tactics, DefaultSkills.Leadership, DefaultSkills.Medicine, DefaultSkills.Scouting })
            {
                TextObject valueText = new TextObject("{=!}{SKILL_VALUE}");
                valueText.SetTextVariable("SKILL_VALUE", hero.GetSkillValue(skill));
                TextObject labeledText = ClanCardSelectionItemPropertyInfo.CreateLabeledValueText(skill.Name, valueText);
                skillsText = skillsText == TextObject.Empty ? labeledText : GameTexts.FindText("str_string_newline_newline_string").SetTextVariable("STR1", skillsText).SetTextVariable("STR2", labeledText);
            }
            yield return new ClanCardSelectionItemPropertyInfo(GameTexts.FindText("str_skills"), skillsText);
        }

        private bool IsFactionMemberAvailableForArmyLeaderChange(Hero hero, Army targetArmy, out TextObject explanation)
        {
            if (hero.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge)
            {
                explanation = new TextObject("{=HAo6iIda}{HERO.NAME} is too young to lead an army.");
                explanation.SetCharacterProperties("HERO", hero.CharacterObject);
                return false;
            }
            if (hero.PartyBelongedTo.Army != null)
            {
                if (hero.PartyBelongedTo.Army.LeaderParty.LeaderHero == hero)
                {
                    explanation = new TextObject("{=kNW1qYSi}{HERO.NAME} is already leading another army.");
                    explanation.SetCharacterProperties("HERO", hero.CharacterObject);
                    return false;
                }
            }
            if (hero.IsPrisoner)
            {
                explanation = new TextObject("{=hv1ARuaU}{HERO.NAME} is in prison right now.");
                explanation.SetCharacterProperties("HERO", hero.CharacterObject);
                return false;
            }
            if (hero.IsFugitive || hero.IsDisabled)
            {
                explanation = new TextObject("{=nMmYZ3xi}{HERO.NAME} is not available right now.");
                explanation.SetCharacterProperties("HERO", hero.CharacterObject);
                return false;
            }
            if (targetArmy.Parties.Count == 1 && targetArmy.LeaderParty.LeaderHero != null)
            {
                explanation = new TextObject("{=pwuEqegC}Army leader is the only member of the army right now.");
                return false;
            }
            if (targetArmy.LeaderParty.MapEvent != null)
            {
                explanation = new TextObject("{=aXPDpkAR}You can not perform this action while the army is in a map event.");
                return false;
            }
            if (hero.CurrentSettlement != null && (hero.CurrentSettlement.IsUnderSiege || hero.CurrentSettlement.IsUnderRaid))
            {
                explanation = new TextObject("{=L9nn40qu}{HERO.NAME}{.o} location is under attack right now.");
                explanation.SetCharacterProperties("HERO", hero.CharacterObject);
                return false;
            }
            explanation = TextObject.Empty;
            return true;
        }

        private void OnChangeLeaderOver(List<object> selectedItems, Action closePopup)
        {
            if (selectedItems.Count == 1)
            {
                Hero newLeader = selectedItems.FirstOrDefault() as Hero;
                Army originalArmy = CurrentSelectedArmy.Army;
                if (newLeader?.CharacterObject != null)
                {
                    StringHelpers.SetCharacterProperties("LEADER", newLeader.CharacterObject);
                }
                ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -TotalArmyCost);
                newLeader.Clan.Kingdom.CreateArmy(newLeader, (from p in originalArmy.Parties where !p.IsMainParty select p).ToList(), newLeader.PartyBelongedTo.Army == null || newLeader.PartyBelongedTo.Army != originalArmy);
                if (originalArmy.LeaderParty != MobileParty.MainParty && originalArmy.LeaderParty.ActualClan != newLeader.Clan)
                {
                    ChangeClanInfluenceAction.Apply(originalArmy.LeaderParty.ActualClan, (TotalArmyCost - (Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, originalArmy.LeaderParty) * 2)) / 2);
                }
                originalArmy.GetType().GetMethod("DisperseInternal", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(originalArmy, new object[] { Army.ArmyDispersionReason.LeaderPartyRemoved });
                RefreshArmyList();
            }
            closePopup?.Invoke();
        }

        private void OnSplitArmyLeaderSelectOver(List<object> selectedItems, Action closePopup)
        {
            if (selectedItems.Count == 1)
            {
                _kingdomInterface.ShowInterface(RefreshArmyList, selectedItems.FirstOrDefault() as Hero);
            }
            closePopup?.Invoke();
        }

        private void ExecuteManageArmy()
        {
            _kingdomInterface.ShowInterface(RefreshArmyList);
        }

        private void ExecuteShowOnMap()
        {
            if (_gauntlet != null && CurrentSelectedArmy != null)
            {
                Vec2 position2D = CurrentSelectedArmy.Army.LeaderParty.Position2D;
                Game.Current.GameStateManager.PopState(0);
                MapScreen.Instance.FastMoveCameraToPosition(position2D);
            }
        }

        private void ExecuteDisbandCurrentArmy()
        {
            if (CurrentSelectedArmy != null && Hero.MainHero.Clan.Influence >= DisbandCost)
            {
                string text = new TextObject("{=619gtcXb}Are you sure you want to disband this army? This will result in relation loss.").ToString();
                if (CurrentSelectedArmy.IsMainArmy)
                {
                    text = new TextObject("{=kqeA8rjL}Are you sure you want to disband your army?").ToString();

                }
                InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_disband_army").ToString(), text, isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), DisbandCurrentArmy, null));
            }
        }

        private void DisbandCurrentArmy()
        {
            if (CurrentSelectedArmy != null && Hero.MainHero.Clan.Influence >= DisbandCost)
            {
                if (DisbandCost > 0)
                {
                    DisbandArmyAction.ApplyByReleasedByPlayerAfterBattle(CurrentSelectedArmy.Army);
                }
                else
                {
                    DisbandArmyAction.ApplyByLeaderPartyRemoved(CurrentSelectedArmy.Army);
                }
                RefreshArmyList();
            }
        }

        [DataSourceProperty]
        public string NoMercenarySelectedText
        {
            get
            {
                return _noMercenarySelectedText;
            }
            set
            {
                if (value != _noMercenarySelectedText)
                {
                    _noMercenarySelectedText = value;
                    OnPropertyChangedWithValue(value, "NoMercenarySelectedText");
                }
            }
        }

        [DataSourceProperty]
        public string DisbandActionExplanationText
        {
            get
            {
                return _disbandActionExplanationText;
            }
            set
            {
                if (value != _disbandActionExplanationText)
                {
                    _disbandActionExplanationText = value;
                    OnPropertyChangedWithValue(value, "DisbandActionExplanationText");
                }
            }
        }

        [DataSourceProperty]
        public string ManageActionExplanationText
        {
            get
            {
                return _manageActionExplanationText;
            }
            set
            {
                if (value != _manageActionExplanationText)
                {
                    _manageActionExplanationText = value;
                    OnPropertyChangedWithValue(value, "ManageActionExplanationText");
                }
            }
        }

        [DataSourceProperty]
        public string HireActionExplanationText
        {
            get
            {
                return _hireActionExplanationText;
            }
            set
            {
                if (value != _hireActionExplanationText)
                {
                    _hireActionExplanationText = value;
                    OnPropertyChangedWithValue(value, "HireActionExplanationText");
                }
            }
        }

        [DataSourceProperty]
        public string ExtendActionExplanationText
        {
            get
            {
                return _extendActionExplanationText;
            }
            set
            {
                if (value != _extendActionExplanationText)
                {
                    _extendActionExplanationText = value;
                    OnPropertyChangedWithValue(value, "ExtendActionExplanationText");
                }
            }
        }

        [DataSourceProperty]
        public string FireActionExplanationText
        {
            get
            {
                return _fireActionExplanationText;
            }
            set
            {
                if (value != _fireActionExplanationText)
                {
                    _fireActionExplanationText = value;
                    OnPropertyChangedWithValue(value, "FireActionExplanationText");
                }
            }
        }

        [DataSourceProperty]
        public KingdomArmyItemVM CurrentSelectedArmy
        {
            get
            {
                return _currentSelectedArmy;
            }
            set
            {
                if (value != _currentSelectedArmy)
                {
                    _currentSelectedArmy = value;
                    OnPropertyChangedWithValue(value, "CurrentSelectedArmy");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel ChangeLeaderHint
        {
            get
            {
                return _changeLeaderHint;
            }
            set
            {
                if (value != _changeLeaderHint)
                {
                    _changeLeaderHint = value;
                    OnPropertyChangedWithValue(value, "ChangeLeaderHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel SplitArmyHint
        {
            get
            {
                return _splitArmyHint;
            }
            set
            {
                if (value != _splitArmyHint)
                {
                    _splitArmyHint = value;
                    OnPropertyChangedWithValue(value, "SplitArmyHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel ManageArmyHint
        {
            get
            {
                return _manageArmyHint;
            }
            set
            {
                if (value != _manageArmyHint)
                {
                    _manageArmyHint = value;
                    OnPropertyChangedWithValue(value, "ManageArmyHint");
                }
            }
        }

        [DataSourceProperty]
        public string MilitaryText
        {
            get
            {
                return _militaryText;
            }
            set
            {
                if (value != _militaryText)
                {
                    _militaryText = value;
                    OnPropertyChangedWithValue(value, "MilitaryText");
                }
            }
        }

        [DataSourceProperty]
        public string ArmiesText
        {
            get
            {
                return _armiesText;
            }
            set
            {
                if (value != _armiesText)
                {
                    _armiesText = value;
                    OnPropertyChangedWithValue(value, "ArmiesText");
                }
            }
        }

        [DataSourceProperty]
        public string MercenariesText
        {
            get
            {
                return _mercenariesText;
            }
            set
            {
                if (value != _mercenariesText)
                {
                    _mercenariesText = value;
                    OnPropertyChangedWithValue(value, "MercenariesText");
                }
            }
        }

        [DataSourceProperty]
        public string PartiesText
        {
            get
            {
                return _partiesText;
            }
            set
            {
                if (value != _partiesText)
                {
                    _partiesText = value;
                    OnPropertyChangedWithValue(value, "PartiesText");
                }
            }
        }

        [DataSourceProperty]
        public string HireText
        {
            get
            {
                return _hireText;
            }
            set
            {
                if (value != _hireText)
                {
                    _hireText = value;
                    OnPropertyChanged("HireText");
                }
            }
        }

        [DataSourceProperty]
        public string ExtendText
        {
            get
            {
                return _extendText;
            }
            set
            {
                if (value != _extendText)
                {
                    _extendText = value;
                    OnPropertyChanged("ExtendText");
                }
            }
        }

        [DataSourceProperty]
        public string FireText
        {
            get
            {
                return _fireText;
            }
            set
            {
                if (value != _fireText)
                {
                    _fireText = value;
                    OnPropertyChanged("FireText");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<KingdomArmyItemVM> Armies
        {
            get
            {
                return _armies;
            }
            set
            {
                if (value != _armies)
                {
                    _armies = value;
                    OnPropertyChangedWithValue(value, "Armies");
                }
            }
        }

        [DataSourceProperty]
        public bool CanDisbandCurrentArmy
        {
            get
            {
                return _canDisbandCurrentArmy;
            }
            set
            {
                if (value != _canDisbandCurrentArmy)
                {
                    _canDisbandCurrentArmy = value;
                    OnPropertyChangedWithValue(value, "CanDisbandCurrentArmy");
                }
            }
        }

        [DataSourceProperty]
        public bool CanManageCurrentArmy
        {
            get
            {
                return _canManageCurrentArmy;
            }
            set
            {
                if (value != _canManageCurrentArmy)
                {
                    _canManageCurrentArmy = value;
                    OnPropertyChangedWithValue(value, "CanManageCurrentArmy");
                }
            }
        }

        [DataSourceProperty]
        public bool CanShowLocationOfCurrentArmy
        {
            get
            {
                return _canShowLocationOfCurrentArmy;
            }
            set
            {
                if (value != _canShowLocationOfCurrentArmy)
                {
                    _canShowLocationOfCurrentArmy = value;
                    OnPropertyChangedWithValue(value, "CanShowLocationOfCurrentArmy");
                }
            }
        }

        [DataSourceProperty]
        public string DisbandText
        {
            get
            {
                return _disbandText;
            }
            set
            {
                if (value != _disbandText)
                {
                    _disbandText = value;
                    OnPropertyChangedWithValue(value, "DisbandText");
                }
            }
        }

        [DataSourceProperty]
        public string ManageText
        {
            get
            {
                return _manageText;
            }
            set
            {
                if (value != _manageText)
                {
                    _manageText = value;
                    OnPropertyChangedWithValue(value, "ManageText");
                }
            }
        }

        [DataSourceProperty]
        public int DisbandCost
        {
            get
            {
                return _disbandCost;
            }
            set
            {
                if (value != _disbandCost)
                {
                    _disbandCost = value;
                    OnPropertyChangedWithValue(value, "DisbandCost");
                }
            }
        }

        [DataSourceProperty]
        public int MinimumArmyCost
        {
            get
            {
                return _minimumArmyCost;
            }
            set
            {
                if (value != _minimumArmyCost)
                {
                    _minimumArmyCost = value;
                    OnPropertyChangedWithValue(value, "MinimumArmyCost");
                }
            }
        }

        [DataSourceProperty]
        public string MinimumArmyCostLabel
        {
            get
            {
                return _minimumArmyCostLabel;
            }
            set
            {
                if (value != _minimumArmyCostLabel)
                {
                    _minimumArmyCostLabel = value;
                    OnPropertyChangedWithValue(value, "MinimumArmyCostLabel");
                }
            }
        }

        [DataSourceProperty]
        public int TotalArmyCost
        {
            get
            {
                return _totalArmyCost;
            }
            set
            {
                if (value != _totalArmyCost)
                {
                    _totalArmyCost = value;
                    OnPropertyChangedWithValue(value, "TotalArmyCost");
                }
            }
        }

        [DataSourceProperty]
        public string TotalArmyCostLabel
        {
            get
            {
                return _totalArmyCostLabel;
            }
            set
            {
                if (value != _totalArmyCostLabel)
                {
                    _totalArmyCostLabel = value;
                    OnPropertyChangedWithValue(value, "TotalArmyCostLabel");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel DisbandHint
        {
            get
            {
                return _disbandHint;
            }
            set
            {
                if (value != _disbandHint)
                {
                    _disbandHint = value;
                    OnPropertyChangedWithValue(value, "DisbandHint");
                }
            }
        }

        [DataSourceProperty]
        public bool ShowArmies
        {
            get
            {
                return _showArmies;
            }
            set
            {
                if (value != _showArmies)
                {
                    _showArmies = value;
                    OnPropertyChangedWithValue(value, "ShowArmies");
                }
            }
        }

        [DataSourceProperty]
        public bool ShowMercenaries
        {
            get
            {
                return _showMercenaries;
            }
            set
            {
                if (value != _showMercenaries)
                {
                    _showMercenaries = value;
                    OnPropertyChangedWithValue(value, "ShowMercenaries");
                }
            }
        }

        [DataSourceProperty]
        public ClanCardSelectionPopupVM CardSelectionPopup
        {
            get
            {
                return _cardSelectionPopup;
            }
            set
            {
                if (value != _cardSelectionPopup)
                {
                    _cardSelectionPopup = value;
                    OnPropertyChangedWithValue(value, "CardSelectionPopup");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<KingdomMercenaryItemVM> Mercenaries
        {
            get
            {
                return _mercenaries;
            }
            set
            {
                if (value != _mercenaries)
                {
                    _mercenaries = value;
                    OnPropertyChangedWithValue(value, "Mercenaries");
                }
            }
        }

        [DataSourceProperty]
        public KingdomMercenaryItemVM CurrentSelectedMercenary
        {
            get
            {
                return _currentSelectedMercenary;
            }
            set
            {
                if (value != _currentSelectedMercenary)
                {
                    _currentSelectedMercenary = value;
                    OnPropertyChangedWithValue(value, "CurrentSelectedMercenary");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel HireHint
        {
            get
            {
                return _hireHint;
            }
            set
            {
                if (value != _hireHint)
                {
                    _hireHint = value;
                    OnPropertyChangedWithValue(value, "HireHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel ExtendHint
        {
            get
            {
                return _extendHint;
            }
            set
            {
                if (value != _extendHint)
                {
                    _extendHint = value;
                    OnPropertyChangedWithValue(value, "ExtendHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel FireHint
        {
            get
            {
                return _fireHint;
            }
            set
            {
                if (value != _fireHint)
                {
                    _fireHint = value;
                    OnPropertyChangedWithValue(value, "FireHint");
                }
            }
        }

        [DataSourceProperty]
        public bool CanChangeCurrentArmyLeader
        {
            get
            {
                return _canChangeCurrentArmyLeader;
            }
            set
            {
                if (value != _canChangeCurrentArmyLeader)
                {
                    _canChangeCurrentArmyLeader = value;
                    OnPropertyChangedWithValue(value, "CanChangeCurrentArmyLeader");
                }
            }
        }

        [DataSourceProperty]
        public bool CanSplitCurrentArmy
        {
            get
            {
                return _canSplitCurrentArmy;
            }
            set
            {
                if (value != _canSplitCurrentArmy)
                {
                    _canSplitCurrentArmy = value;
                    OnPropertyChangedWithValue(value, "CanSplitCurrentArmy");
                }
            }
        }

        [DataSourceProperty]
        public bool CanHireCurrentMercenary
        {
            get
            {
                return _canHireCurrentMercenary;
            }
            set
            {
                if (value != _canHireCurrentMercenary)
                {
                    _canHireCurrentMercenary = value;
                    OnPropertyChangedWithValue(value, "CanHireCurrentMercenary");
                }
            }
        }

        [DataSourceProperty]
        public bool CanExtendCurrentMercenary
        {
            get
            {
                return _canExtendCurrentMercenary;
            }
            set
            {
                if (value != _canExtendCurrentMercenary)
                {
                    _canExtendCurrentMercenary = value;
                    OnPropertyChangedWithValue(value, "CanExtendCurrentMercenary");
                }
            }
        }

        [DataSourceProperty]
        public bool ShouldExtendCurrentMercenary
        {
            get
            {
                return _shouldExtendCurrentMercenary;
            }
            set
            {
                if (value != _shouldExtendCurrentMercenary)
                {
                    _shouldExtendCurrentMercenary = value;
                    OnPropertyChangedWithValue(value, "ShouldExtendCurrentMercenary");
                }
            }
        }

        [DataSourceProperty]
        public bool CanFireCurrentMercenary
        {
            get
            {
                return _canFireCurrentMercenary;
            }
            set
            {
                if (value != _canFireCurrentMercenary)
                {
                    _canFireCurrentMercenary = value;
                    OnPropertyChangedWithValue(value, "CanFireCurrentMercenary");
                }
            }
        }

        [DataSourceProperty]
        public bool IsAcceptableMercenarySelected
        {
            get
            {
                return _isAcceptableMercenarySelected;
            }
            set
            {
                if (value != _isAcceptableMercenarySelected)
                {
                    _isAcceptableMercenarySelected = value;
                    OnPropertyChangedWithValue(value, "IsAcceptableMercenarySelected");
                }
            }
        }
        [DataSourceProperty]
        public int HireCost
        {
            get
            {
                return _hireCost;
            }
            set
            {
                if (value != _hireCost)
                {
                    _hireCost = value;
                    OnPropertyChangedWithValue(value, "HireCost");
                }
            }

        }
        [DataSourceProperty]
        public string HireCostLabel
        {
            get
            {
                return _hireCostLabel;
            }
            set
            {
                if (value != _hireCostLabel)
                {
                    _hireCostLabel = value;
                    OnPropertyChangedWithValue(value, "HireCostLabel");
                }
            }
        }
        [DataSourceProperty]
        public string RemainingContractTimeLabel
        {
            get
            {
                return _remainingContractTimeLabel;
            }
            set
            {
                if (value != _remainingContractTimeLabel)
                {
                    _remainingContractTimeLabel = value;
                    OnPropertyChangedWithValue(value, "RemainingContractTimeLabel");
                }
            }
        }
    }
}