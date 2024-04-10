﻿using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using Warfare.Helpers;

namespace Warfare.ViewModels.ArmyManagement
{
    internal class KingdomArmyManagementVM : ViewModel
    {
        private readonly Action _onClose;

        private readonly Action _onFinalize;

        public readonly Army Army;

        private readonly ArmyManagementItemVM _mainPartyItem;

        private readonly float _initialInfluence;

        private string _latestTutorialElementID;

        private string _playerDoesntHaveEnoughInfluenceStr;

        private int _influenceSpentForCohesionBoosting;

        private int _boostedCohesion;

        private string _titleText;

        private string _boostTitleText;

        private string _cancelText;

        private string _doneText;

        private bool _canCreateArmy;

        private bool _canBoostCohesion;

        private List<MobileParty> _currentParties;

        private ArmyManagementItemVM _focusedItem;

        private MBBindingList<ArmyManagementItemVM> _partyList;

        private MBBindingList<ArmyManagementItemVM> _partiesInCart;

        private MBBindingList<ArmyManagementItemVM> _partiesToRemove;

        private ArmyManagementSortControllerVM _sortControllerVM;

        private int _totalStrength;

        private int _totalCost;

        private int _cohesion;

        private int _cohesionBoostCost;

        private string _cohesionText;

        private int _newCohesion;

        private string _totalStrengthText;

        private string _totalCostText;

        private string _totalCostNumbersText;

        private string _totalInfluence;

        private string _totalLords;

        private string _costText;

        private string _strengthText;

        private string _lordsText;

        private string _distanceText;

        private string _clanText;

        private string _ownerText;

        private string _nameText;

        private string _disbandArmyText;

        private string _cohesionBoostAmountText;

        private bool _playerHasArmy;

        private bool _canDisbandArmy;

        private bool _canAffordInfluenceCost;

        private string _moraleText;

        private string _foodText;

        private BasicTooltipViewModel _cohesionHint;

        private HintViewModel _moraleHint;

        private HintViewModel _foodHint;

        private HintViewModel _boostCohesionHint;

        private HintViewModel _disbandArmyHint;

        private HintViewModel _doneHint;

        private int _disbandCost;

        public ElementNotificationVM _tutorialNotification;

        private InputKeyItemVM _resetInputKey;

        private InputKeyItemVM _cancelInputKey;

        private InputKeyItemVM _doneInputKey;

        private InputKeyItemVM _removeInputKey;

        public KingdomArmyManagementVM(Action onClose, Action onFinalize)
        {
            _onClose = onClose;
            _onFinalize = onFinalize;
            PartyList = new();
            PartiesInCart = new();
            _partiesToRemove = new();
            _currentParties = new();
            CohesionHint = new();
            FoodHint = new();
            MoraleHint = new();
            BoostCohesionHint = new();
            DisbandArmyHint = new();
            DoneHint = new();
            TutorialNotification = new();
            CanAffordInfluenceCost = true;
            Army = VMHelper.Military!.CurrentSelectedArmy.Army;
            PlayerHasArmy = Army != null;
            foreach (MobileParty item in MobileParty.All)
            {
                if (item.LeaderHero != null && item.MapFaction == Hero.MainHero.MapFaction && item.LeaderHero != Army.LeaderParty.LeaderHero && !item.IsCaravan)
                {
                    PartyList.Add(new ArmyManagementItemVM(OnAddToCart, OnRemove, OnFocus, item));
                }
            }
            _mainPartyItem = new ArmyManagementItemVM(null, null, null, Army.LeaderParty)
            {
                IsTransferDisabled = true,
                IsAlreadyWithPlayer = true,
                IsMainHero = true,
                IsInCart = true
            };
            PartiesInCart.Add(_mainPartyItem);
            foreach (ArmyManagementItemVM party in PartyList)
            {
                if (Army != null && party.Party.Army == Army && party.Party != Army.LeaderParty)
                {
                    party.Cost = 0;
                    party.IsAlreadyWithPlayer = true;
                    party.IsInCart = true;
                    PartiesInCart.Add(party);
                }
            }
            CalculateCohesion();
            CanBoostCohesion = PlayerHasArmy && NewCohesion < 100;
            if (Army != null)
            {
                CohesionBoostCost = Campaign.Current.Models.ArmyManagementCalculationModel.GetCohesionBoostInfluenceCost(Army, 10);
            }
            _initialInfluence = Hero.MainHero.Clan.Influence;
            OnRefresh();
            Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
            SortControllerVM = new ArmyManagementSortControllerVM(_partyList);
            Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(OnTutorialNotificationElementIDChange);
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            TitleText = GameTexts.FindText("str_army_management").ToString();
            BoostTitleText = GameTexts.FindText("str_boost_cohesion").ToString();
            CancelText = GameTexts.FindText("str_cancel").ToString();
            DoneText = GameTexts.FindText("str_done").ToString();
            DistanceText = GameTexts.FindText("str_distance").ToString();
            CostText = GameTexts.FindText("str_cost").ToString();
            StrengthText = GameTexts.FindText("str_men_numbersign").ToString();
            LordsText = GameTexts.FindText("str_leader").ToString();
            ClanText = GameTexts.FindText("str_clans").ToString();
            NameText = GameTexts.FindText("str_sort_by_name_label").ToString();
            OwnerText = GameTexts.FindText("str_party").ToString();
            DisbandArmyText = GameTexts.FindText("str_disband_army").ToString();
            DisbandCost = Army.LeaderParty == MobileParty.MainParty ? 0 : Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfDisbandingArmy();
            CanDisbandArmy = GetCanDisbandArmyWithReason(out var disabledReason);
            DisbandArmyHint.HintText = disabledReason;
            _playerDoesntHaveEnoughInfluenceStr = GameTexts.FindText("str_warning_you_dont_have_enough_influence").ToString();
            GameTexts.SetVariable("TOTAL_INFLUENCE", MathF.Round(Hero.MainHero.Clan.Influence));
            TotalInfluence = GameTexts.FindText("str_total_influence").ToString();
            GameTexts.SetVariable("NUMBER", 10);
            CohesionBoostAmountText = GameTexts.FindText("str_plus_with_number").ToString();
            PartyList.ApplyActionOnAllItems(delegate (ArmyManagementItemVM x)
            {
                x.RefreshValues();
            });
            PartiesInCart.ApplyActionOnAllItems(delegate (ArmyManagementItemVM x)
            {
                x.RefreshValues();
            });
            TutorialNotification.RefreshValues();
        }

        public void OnFrameTick(GauntletLayer gauntlet)
        {
            if (gauntlet.Input.IsHotKeyDownAndReleased("Exit"))
            {
                ExecuteCancel();
            }
            else if (gauntlet.Input.IsHotKeyDownAndReleased("Confirm"))
            {
                ExecuteDone();
            }
            else if (gauntlet.Input.IsHotKeyDownAndReleased("Reset"))
            {
                ExecuteReset();
            }
            else if (gauntlet.Input.IsHotKeyReleased("RemoveParty") && FocusedItem != null)
            {
                FocusedItem.ExecuteAction();
            }
        }

        private void CalculateCohesion()
        {
            if (Army == null)
            {
                return;
            }
            Cohesion = (int)Army.Cohesion;
            NewCohesion = MathF.Min(Cohesion + _boostedCohesion, 100);
            ArmyManagementCalculationModel armyManagementCalculationModel = Campaign.Current.Models.ArmyManagementCalculationModel;
            _currentParties.Clear();
            foreach (ArmyManagementItemVM item in PartiesInCart)
            {
                if (item.Party != Army.LeaderParty)
                {
                    _currentParties.Add(item.Party);
                    if (!item.IsAlreadyWithPlayer)
                    {
                        NewCohesion = armyManagementCalculationModel.CalculateNewCohesion(Army, item.Party.Party, NewCohesion, 1);
                    }
                }
            }
        }

        private void OnFocus(ArmyManagementItemVM focusedItem)
        {
            FocusedItem = focusedItem;
        }

        private void OnAddToCart(ArmyManagementItemVM armyItem)
        {
            if (!PartiesInCart.Contains(armyItem))
            {
                PartiesInCart.Add(armyItem);
                armyItem.IsInCart = true;
                Game.Current.EventManager.TriggerEvent(new PartyAddedToArmyByPlayerEvent(armyItem.Party));
                if (_partiesToRemove.Contains(armyItem))
                {
                    _partiesToRemove.Remove(armyItem);
                }

                if (armyItem.IsAlreadyWithPlayer)
                {
                    armyItem.CanJoinBackWithoutCost = false;
                }

                TotalCost += armyItem.Cost;
            }

            CalculateCohesion();
            OnRefresh();
        }

        private void OnRemove(ArmyManagementItemVM armyItem)
        {
            if (PartiesInCart.Contains(armyItem))
            {
                PartiesInCart.Remove(armyItem);
                armyItem.IsInCart = false;
                _partiesToRemove.Add(armyItem);
                if (armyItem.IsAlreadyWithPlayer)
                {
                    armyItem.CanJoinBackWithoutCost = true;
                }

                TotalCost -= armyItem.Cost;
            }

            CalculateCohesion();
            OnRefresh();
        }

        private void ApplyCohesionChange()
        {
            if (Army != null)
            {
                int num = NewCohesion - Cohesion;
                if (_influenceSpentForCohesionBoosting > 0 || Army.Parties.All((MobileParty p) => p.ActualClan == Army.LeaderParty.ActualClan))
                {
                    Army.BoostCohesionWithInfluence(num, _influenceSpentForCohesionBoosting);
                }
            }
        }

        private void OnBoostCohesion()
        {
            if (Army != null && Army.Cohesion < 100f)
            {
                if (Hero.MainHero.Clan.Influence >= (float)(CohesionBoostCost + TotalCost))
                {
                    NewCohesion += 10;
                    TotalCost += CohesionBoostCost;
                    _boostedCohesion += 10;
                    _influenceSpentForCohesionBoosting += CohesionBoostCost;
                    OnRefresh();
                }
                else
                {
                    MBInformationManager.AddQuickInformation(new TextObject("{=Xmw93W6a}Not Enough Influence"));
                }
            }
        }

        private void OnRefresh()
        {
            TotalStrength = (from x in PartiesInCart select Campaign.Current.Models.ArmyManagementCalculationModel.GetPartyStrength(x.Party.Party)).Sum();
            GameTexts.SetVariable("LEFT", GameTexts.FindText("str_total_cost").ToString());
            TotalCostText = GameTexts.FindText("str_LEFT_colon").ToString();
            GameTexts.SetVariable("LEFT", TotalCost.ToString());
            GameTexts.SetVariable("RIGHT", ((int)Hero.MainHero.Clan.Influence).ToString());
            TotalCostNumbersText = GameTexts.FindText("str_LEFT_over_RIGHT").ToString();
            GameTexts.SetVariable("NUM", PartiesInCart.Count());
            TotalLords = GameTexts.FindText("str_NUM_lords").ToString();
            GameTexts.SetVariable("LEFT", GameTexts.FindText("str_strength").ToString());
            TotalStrengthText = GameTexts.FindText("str_LEFT_colon").ToString();
            CanCreateArmy = (float)TotalCost <= Hero.MainHero.Clan.Influence && PartiesInCart.Count() > 1;
            PlayerHasArmy = Army != null && (_partiesToRemove.Count <= 0 || PartiesInCart.Count((ArmyManagementItemVM p) => p.IsAlreadyWithPlayer) >= 1);
            CanBoostCohesion = PlayerHasArmy && 100 - NewCohesion >= 10;
            if (CanBoostCohesion)
            {
                TextObject textObject = new TextObject("{=s5b77f0H}Add +{BOOSTAMOUNT} cohesion to your army");
                textObject.SetTextVariable("BOOSTAMOUNT", 10);
                BoostCohesionHint.HintText = new TextObject("{=!}" + textObject.ToString());
            }
            else if (100 - NewCohesion >= 10)
            {
                TextObject textObject2 = new TextObject("{=rsHPaaYZ}Cohesion needs to be lower than {MINAMOUNT} to boost");
                textObject2.SetTextVariable("MINAMOUNT", 90);
                BoostCohesionHint.HintText = new TextObject("{=!}" + textObject2.ToString());
            }
            else
            {
                BoostCohesionHint.HintText = new TextObject("{=Ioiqzz4E}You need to be in an army to boost cohesion");
            }

            if (Army != null)
            {
                CohesionText = GameTexts.FindText("str_cohesion").ToString();
            }

            UpdateTooltips();
            PartiesInCart.Sort(new ArmyManagementVM.ManagementItemComparer());
            CanDisbandArmy = GetCanDisbandArmyWithReason(out var disabledReason);
            DisbandArmyHint.HintText = disabledReason;
        }

        private bool GetCanDisbandArmyWithReason(out TextObject disabledReason)
        {
            if (Army == null)
            {

                disabledReason = new TextObject("{=iSZTOeYH}No army to disband.");
                return false;
            }

            if (Clan.PlayerClan.Influence < DisbandCost)
            {
                disabledReason = GameTexts.FindText("str_warning_you_dont_have_enough_influence");
                return false;
            }

            if (Army.LeaderParty.MapEvent != null)
            {
                disabledReason = new TextObject("{=uipNpzVw}Cannot disband the army right now.");
                return false;
            }
            
            if (Army.LeaderParty.SiegeEvent != null)
            {
                disabledReason = new TextObject("{=mGWrSJLE}You cannot manage an army while it's in a map event.");
                return false;
            }

            if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out var disabledReason2))
            {
                disabledReason = disabledReason2;
                return false;
            }

            disabledReason = TextObject.Empty;
            return true;
        }

        private void UpdateTooltips()
        {
            if (PlayerHasArmy)
            {
                CohesionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetArmyCohesionTooltip(Army));
                Army.RecalculateArmyMorale();
                MathF.Round(Army.Morale, 1).ToString("0.0");
                MBTextManager.SetTextVariable("BASE_EFFECT", MathF.Round(Army.LeaderParty.Morale, 1).ToString("0.0"));
                MBTextManager.SetTextVariable("STR1", "");
                MBTextManager.SetTextVariable("STR2", "");
                MBTextManager.SetTextVariable("ARMY_MORALE", Army.Morale);
                foreach (MobileParty party in Army.Parties)
                {
                    MBTextManager.SetTextVariable("STR1", GameTexts.FindText("str_STR1_STR2").ToString());
                    MBTextManager.SetTextVariable("PARTY_NAME", party.Name);
                    MBTextManager.SetTextVariable("PARTY_MORALE", (int)party.Morale);
                    MBTextManager.SetTextVariable("STR2", GameTexts.FindText("str_new_morale_item_line"));
                }

                MBTextManager.SetTextVariable("ARMY_MORALE_ITEMS", GameTexts.FindText("str_STR1_STR2").ToString());
                MoraleHint.HintText = GameTexts.FindText("str_army_morale_tooltip");
            }
            else
            {
                GameTexts.SetVariable("reg1", (int)Army.LeaderParty.Morale);
                MoraleHint.HintText = GameTexts.FindText("str_morale_reg1");
            }

            DoneHint.HintText = new TextObject("{=!}" + (CanAffordInfluenceCost ? null : _playerDoesntHaveEnoughInfluenceStr));
            MBTextManager.SetTextVariable("newline", "\n");
            MBTextManager.SetTextVariable("DAILY_FOOD_CONSUMPTION", Army.LeaderParty.FoodChange);
            FoodHint.HintText = GameTexts.FindText("str_food_consumption_tooltip");
        }

        public void ExecuteDone()
        {
            if (!CanAffordInfluenceCost)
            {
                return;
            }

            if (NewCohesion > Cohesion)
            {
                ApplyCohesionChange();
            }

            if (PartiesInCart.Count > 1 && MobileParty.MainParty.MapFaction.IsKingdomFaction)
            {
                if (Army == null)
                {
                    ((Kingdom)MobileParty.MainParty.MapFaction).CreateArmy(Hero.MainHero, Hero.MainHero.HomeSettlement, Army.ArmyTypes.Patrolling);
                }

                foreach (ArmyManagementItemVM item in PartiesInCart)
                {
                    if (item.Party != Army.LeaderParty)
                    {
                        item.Party.Army = Army;
                        SetPartyAiAction.GetActionForEscortingParty(item.Party, Army.LeaderParty);
                    }
                }

                ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -(TotalCost - _influenceSpentForCohesionBoosting));
            }

            if (_partiesToRemove.Count > 0)
            {
                bool flag = false;
                foreach (ArmyManagementItemVM item2 in _partiesToRemove)
                {
                    if (item2.Party == Army.LeaderParty)
                    {
                        item2.Party.Army = null;
                        flag = true;
                    }
                }

                if (!flag)
                {
                    foreach (ArmyManagementItemVM item3 in _partiesToRemove)
                    {
                        if (Army?.Parties.Contains(item3.Party) ?? false)
                        {
                            item3.Party.Army = null;
                        }
                    }
                }

                _partiesToRemove.Clear();
            }
            _onFinalize();
            _onClose();
            CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
        }

        public void ExecuteCancel()
        {
            ChangeClanInfluenceAction.Apply(Clan.PlayerClan, _initialInfluence - Clan.PlayerClan.Influence);
            _onClose();
        }

        public void ExecuteReset()
        {
            foreach (ArmyManagementItemVM item in PartiesInCart.ToList())
            {
                OnRemove(item);
                item.UpdateEligibility();
            }

            PartiesInCart.Add(_mainPartyItem);
            foreach (ArmyManagementItemVM party in PartyList)
            {
                if (party.IsAlreadyWithPlayer)
                {
                    PartiesInCart.Add(party);
                    party.IsInCart = true;
                    party.CanJoinBackWithoutCost = false;
                }
            }

            NewCohesion = Cohesion;
            ChangeClanInfluenceAction.Apply(Clan.PlayerClan, _initialInfluence - Clan.PlayerClan.Influence);
            TotalCost = 0;
            _boostedCohesion = 0;
            _influenceSpentForCohesionBoosting = 0;
            _partiesToRemove.Clear();
            OnRefresh();
        }

        public void ExecuteDisbandArmy()
        {
            if (CanDisbandArmy)
            {
                TextObject inquiryText = new TextObject("{=619gtcXb}Are you sure you want to disband this army? This will result in relation loss.");
                if (Army.LeaderParty == MobileParty.MainParty)
                {
                    inquiryText = new TextObject("{=kqeA8rjL}Are you sure you want to disband your army?");
                }
                InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_disband_army").ToString(), inquiryText.ToString(), isAffirmativeOptionShown: true, isNegativeOptionShown: true, GameTexts.FindText("str_yes").ToString(), GameTexts.FindText("str_no").ToString(), delegate
                {
                    DisbandArmy();
                }, null));
            }
        }

        public void ExecuteBoostCohesionManual()
        {
            OnBoostCohesion();
            Game.Current.EventManager.TriggerEvent(new ArmyCohesionBoostedByPlayerEvent());
        }

        private void DisbandArmy()
        {
            foreach (ArmyManagementItemVM item in PartiesInCart.ToList())
            {
                item.IsInCart = false;
            }
            PartiesInCart.Clear();
            _partiesToRemove.Clear();
            if (DisbandCost > 0)
            {
                DisbandArmyAction.ApplyByReleasedByPlayerAfterBattle(Army);
            }
            else
            {
                DisbandArmyAction.ApplyByLeaderPartyRemoved(Army);
            }
            _onFinalize();
            _onClose();
        }

        private void OnCloseBoost()
        {
            Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
        }

        private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
        {
            if (obj.NewNotificationElementID != _latestTutorialElementID)
            {
                if (_latestTutorialElementID != null)
                {
                    TutorialNotification.ElementID = string.Empty;
                }

                _latestTutorialElementID = obj.NewNotificationElementID;
                if (_latestTutorialElementID != null)
                {
                    TutorialNotification.ElementID = _latestTutorialElementID;
                }
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(OnTutorialNotificationElementIDChange);
            CancelInputKey?.OnFinalize();
            DoneInputKey?.OnFinalize();
            ResetInputKey?.OnFinalize();
            RemoveInputKey?.OnFinalize();
        }

        public void SetResetInputKey(HotKey hotKey)
        {
            ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, isConsoleOnly: true);
        }

        public void SetCancelInputKey(HotKey hotKey)
        {
            CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, isConsoleOnly: true);
        }

        public void SetDoneInputKey(HotKey hotKey)
        {
            DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, isConsoleOnly: true);
        }

        public void SetRemoveInputKey(HotKey hotKey)
        {
            RemoveInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, isConsoleOnly: true);
        }

        [DataSourceProperty]
        public ElementNotificationVM TutorialNotification
        {
            get
            {
                return _tutorialNotification;
            }
            set
            {
                if (value != _tutorialNotification)
                {
                    _tutorialNotification = value;
                    OnPropertyChangedWithValue(value, "TutorialNotification");
                }
            }
        }

        [DataSourceProperty]
        public ArmyManagementSortControllerVM SortControllerVM
        {
            get
            {
                return _sortControllerVM;
            }
            set
            {
                if (value != _sortControllerVM)
                {
                    _sortControllerVM = value;
                    OnPropertyChangedWithValue(value, "SortControllerVM");
                }
            }
        }

        [DataSourceProperty]
        public string BoostTitleText
        {
            get
            {
                return _boostTitleText;
            }
            set
            {
                if (value != _boostTitleText)
                {
                    _boostTitleText = value;
                    OnPropertyChangedWithValue(value, "BoostTitleText");
                }
            }
        }

        [DataSourceProperty]
        public string DisbandArmyText
        {
            get
            {
                return _disbandArmyText;
            }
            set
            {
                if (value != _disbandArmyText)
                {
                    _disbandArmyText = value;
                    OnPropertyChangedWithValue(value, "DisbandArmyText");
                }
            }
        }

        [DataSourceProperty]
        public string CohesionBoostAmountText
        {
            get
            {
                return _cohesionBoostAmountText;
            }
            set
            {
                if (value != _cohesionBoostAmountText)
                {
                    _cohesionBoostAmountText = value;
                    OnPropertyChangedWithValue(value, "CohesionBoostAmountText");
                }
            }
        }

        [DataSourceProperty]
        public string DistanceText
        {
            get
            {
                return _distanceText;
            }
            set
            {
                if (value != _distanceText)
                {
                    _distanceText = value;
                    OnPropertyChangedWithValue(value, "DistanceText");
                }
            }
        }

        [DataSourceProperty]
        public string CostText
        {
            get
            {
                return _costText;
            }
            set
            {
                if (value != _costText)
                {
                    _costText = value;
                    OnPropertyChangedWithValue(value, "CostText");
                }
            }
        }

        [DataSourceProperty]
        public string OwnerText
        {
            get
            {
                return _ownerText;
            }
            set
            {
                if (value != _ownerText)
                {
                    _ownerText = value;
                    OnPropertyChangedWithValue(value, "OwnerText");
                }
            }
        }

        [DataSourceProperty]
        public string StrengthText
        {
            get
            {
                return _strengthText;
            }
            set
            {
                if (value != _strengthText)
                {
                    _strengthText = value;
                    OnPropertyChangedWithValue(value, "StrengthText");
                }
            }
        }

        [DataSourceProperty]
        public string LordsText
        {
            get
            {
                return _lordsText;
            }
            set
            {
                if (value != _lordsText)
                {
                    _lordsText = value;
                    OnPropertyChangedWithValue(value, "LordsText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalInfluence
        {
            get
            {
                return _totalInfluence;
            }
            set
            {
                if (value != _totalInfluence)
                {
                    _totalInfluence = value;
                    OnPropertyChangedWithValue(value, "TotalInfluence");
                }
            }
        }

        [DataSourceProperty]
        public int TotalStrength
        {
            get
            {
                return _totalStrength;
            }
            set
            {
                if (value != _totalStrength)
                {
                    _totalStrength = value;
                    OnPropertyChangedWithValue(value, "TotalStrength");
                }
            }
        }

        [DataSourceProperty]
        public int TotalCost
        {
            get
            {
                return _totalCost;
            }
            set
            {
                if (value != _totalCost)
                {
                    _totalCost = value;
                    CanAffordInfluenceCost = TotalCost <= 0 || (float)TotalCost <= Hero.MainHero.Clan.Influence;
                    OnPropertyChangedWithValue(value, "TotalCost");
                }
            }
        }

        [DataSourceProperty]
        public string TotalLords
        {
            get
            {
                return _totalLords;
            }
            set
            {
                if (value != _totalLords)
                {
                    _totalLords = value;
                    OnPropertyChangedWithValue(value, "TotalLords");
                }
            }
        }

        [DataSourceProperty]
        public bool CanCreateArmy
        {
            get
            {
                return _canCreateArmy;
            }
            set
            {
                if (value != _canCreateArmy)
                {
                    _canCreateArmy = value;
                    OnPropertyChangedWithValue(value, "CanCreateArmy");
                }
            }
        }

        [DataSourceProperty]
        public bool CanBoostCohesion
        {
            get
            {
                return _canBoostCohesion;
            }
            set
            {
                if (value != _canBoostCohesion)
                {
                    _canBoostCohesion = value;
                    OnPropertyChangedWithValue(value, "CanBoostCohesion");
                }
            }
        }

        [DataSourceProperty]
        public bool CanDisbandArmy
        {
            get
            {
                return _canDisbandArmy;
            }
            set
            {
                if (value != _canDisbandArmy)
                {
                    _canDisbandArmy = value;
                    OnPropertyChangedWithValue(value, "CanDisbandArmy");
                }
            }
        }

        [DataSourceProperty]
        public bool CanAffordInfluenceCost
        {
            get
            {
                return _canAffordInfluenceCost;
            }
            set
            {
                if (value != _canAffordInfluenceCost)
                {
                    _canAffordInfluenceCost = value;
                    OnPropertyChangedWithValue(value, "CanAffordInfluenceCost");
                }
            }
        }

        [DataSourceProperty]
        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                if (value != _titleText)
                {
                    _titleText = value;
                    OnPropertyChangedWithValue(value, "TitleText");
                }
            }
        }

        [DataSourceProperty]
        public string ClanText
        {
            get
            {
                return _clanText;
            }
            set
            {
                if (value != _clanText)
                {
                    _clanText = value;
                    OnPropertyChangedWithValue(value, "ClanText");
                }
            }
        }

        [DataSourceProperty]
        public string NameText
        {
            get
            {
                return _nameText;
            }
            set
            {
                if (value != _nameText)
                {
                    _nameText = value;
                    OnPropertyChangedWithValue(value, "NameText");
                }
            }
        }

        [DataSourceProperty]
        public string CancelText
        {
            get
            {
                return _cancelText;
            }
            set
            {
                if (value != _cancelText)
                {
                    _cancelText = value;
                    OnPropertyChangedWithValue(value, "CancelText");
                }
            }
        }

        [DataSourceProperty]
        public string DoneText
        {
            get
            {
                return _doneText;
            }
            set
            {
                if (value != _doneText)
                {
                    _doneText = value;
                    OnPropertyChangedWithValue(value, "DoneText");
                }
            }
        }

        [DataSourceProperty]
        public ArmyManagementItemVM FocusedItem
        {
            get
            {
                return _focusedItem;
            }
            set
            {
                if (value != _focusedItem)
                {
                    _focusedItem = value;
                    OnPropertyChangedWithValue(value, "FocusedItem");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<ArmyManagementItemVM> PartyList
        {
            get
            {
                return _partyList;
            }
            set
            {
                if (value != _partyList)
                {
                    _partyList = value;
                    OnPropertyChangedWithValue(value, "PartyList");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<ArmyManagementItemVM> PartiesInCart
        {
            get
            {
                return _partiesInCart;
            }
            set
            {
                if (value != _partiesInCart)
                {
                    _partiesInCart = value;
                    OnPropertyChangedWithValue(value, "PartiesInCart");
                }
            }
        }

        [DataSourceProperty]
        public string TotalStrengthText
        {
            get
            {
                return _totalStrengthText;
            }
            set
            {
                if (value != _totalStrengthText)
                {
                    _totalStrengthText = value;
                    OnPropertyChangedWithValue(value, "TotalStrengthText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalCostText
        {
            get
            {
                return _totalCostText;
            }
            set
            {
                if (value != _totalCostText)
                {
                    _totalCostText = value;
                    OnPropertyChangedWithValue(value, "TotalCostText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalCostNumbersText
        {
            get
            {
                return _totalCostNumbersText;
            }
            set
            {
                if (value != _totalCostNumbersText)
                {
                    _totalCostNumbersText = value;
                    OnPropertyChangedWithValue(value, "TotalCostNumbersText");
                }
            }
        }

        [DataSourceProperty]
        public string CohesionText
        {
            get
            {
                return _cohesionText;
            }
            set
            {
                if (value != _cohesionText)
                {
                    _cohesionText = value;
                    OnPropertyChangedWithValue(value, "CohesionText");
                }
            }
        }

        [DataSourceProperty]
        public int Cohesion
        {
            get
            {
                return _cohesion;
            }
            set
            {
                if (value != _cohesion)
                {
                    _cohesion = value;
                    OnPropertyChangedWithValue(value, "Cohesion");
                }
            }
        }

        [DataSourceProperty]
        public int CohesionBoostCost
        {
            get
            {
                return _cohesionBoostCost;
            }
            set
            {
                if (value != _cohesionBoostCost)
                {
                    _cohesionBoostCost = value;
                    OnPropertyChangedWithValue(value, "CohesionBoostCost");
                }
            }
        }

        [DataSourceProperty]
        public bool PlayerHasArmy
        {
            get
            {
                return _playerHasArmy;
            }
            set
            {
                if (value != _playerHasArmy)
                {
                    _playerHasArmy = value;
                    OnPropertyChangedWithValue(value, "PlayerHasArmy");
                }
            }
        }

        [DataSourceProperty]
        public string MoraleText
        {
            get
            {
                return _moraleText;
            }
            set
            {
                if (value != _moraleText)
                {
                    _moraleText = value;
                    OnPropertyChangedWithValue(value, "MoraleText");
                }
            }
        }

        [DataSourceProperty]
        public string FoodText
        {
            get
            {
                return _foodText;
            }
            set
            {
                if (value != _foodText)
                {
                    _foodText = value;
                    OnPropertyChangedWithValue(value, "FoodText");
                }
            }
        }

        [DataSourceProperty]
        public int NewCohesion
        {
            get
            {
                return _newCohesion;
            }
            set
            {
                if (value != _newCohesion)
                {
                    _newCohesion = value;
                    OnPropertyChangedWithValue(value, "NewCohesion");
                }
            }
        }

        [DataSourceProperty]
        public BasicTooltipViewModel CohesionHint
        {
            get
            {
                return _cohesionHint;
            }
            set
            {
                if (value != _cohesionHint)
                {
                    _cohesionHint = value;
                    OnPropertyChangedWithValue(value, "CohesionHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel MoraleHint
        {
            get
            {
                return _moraleHint;
            }
            set
            {
                if (value != _moraleHint)
                {
                    _moraleHint = value;
                    OnPropertyChangedWithValue(value, "MoraleHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel BoostCohesionHint
        {
            get
            {
                return _boostCohesionHint;
            }
            set
            {
                if (value != _boostCohesionHint)
                {
                    _boostCohesionHint = value;
                    OnPropertyChangedWithValue(value, "BoostCohesionHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel DisbandArmyHint
        {
            get
            {
                return _disbandArmyHint;
            }
            set
            {
                if (value != _disbandArmyHint)
                {
                    _disbandArmyHint = value;
                    OnPropertyChangedWithValue(value, "DisbandArmyHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel DoneHint
        {
            get
            {
                return _doneHint;
            }
            set
            {
                if (value != _doneHint)
                {
                    _doneHint = value;
                    OnPropertyChangedWithValue(value, "DoneHint");
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel FoodHint
        {
            get
            {
                return _foodHint;
            }
            set
            {
                if (value != _foodHint)
                {
                    _foodHint = value;
                    OnPropertyChangedWithValue(value, "FoodHint");
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
        public InputKeyItemVM ResetInputKey
        {
            get
            {
                return _resetInputKey;
            }
            set
            {
                if (value != _resetInputKey)
                {
                    _resetInputKey = value;
                    OnPropertyChangedWithValue(value, "ResetInputKey");
                }
            }
        }

        [DataSourceProperty]
        public InputKeyItemVM CancelInputKey
        {
            get
            {
                return _cancelInputKey;
            }
            set
            {
                if (value != _cancelInputKey)
                {
                    _cancelInputKey = value;
                    OnPropertyChangedWithValue(value, "CancelInputKey");
                }
            }
        }

        [DataSourceProperty]
        public InputKeyItemVM DoneInputKey
        {
            get
            {
                return _doneInputKey;
            }
            set
            {
                if (value != _doneInputKey)
                {
                    _doneInputKey = value;
                    OnPropertyChangedWithValue(value, "DoneInputKey");
                }
            }
        }

        [DataSourceProperty]
        public InputKeyItemVM RemoveInputKey
        {
            get
            {
                return _removeInputKey;
            }
            set
            {
                if (value == _removeInputKey)
                {
                    return;
                }

                _removeInputKey = value;
                OnPropertyChangedWithValue(value, "RemoveInputKey");
                foreach (ArmyManagementItemVM party in PartyList)
                {
                    party.RemoveInputKey = value;
                }
            }
        }

        [DataSourceProperty]
        public bool IsDisbandWithCost
        {
            get
            {
                return Army.LeaderParty == null || Army.LeaderParty == MobileParty.MainParty;
            }
        }
    }
}