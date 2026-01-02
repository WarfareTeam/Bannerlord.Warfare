using System;
using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Warfare.Extensions;
using Warfare.Helpers;

namespace Warfare.ViewModels.ArmyManagement
{
    internal class WarfareArmyManagementVM : ViewModel
    {
        public class ManagementItemComparer : IComparer<WarfareArmyManagementItemVM>
        {
            public int Compare(WarfareArmyManagementItemVM x, WarfareArmyManagementItemVM y)
            {
                if (x.IsMainHero)
                {
                    return -1;
                }

                return y.IsAlreadyWithPlayer.CompareTo(x.IsAlreadyWithPlayer);
            }
        }

        private readonly Action _onClose;

        private readonly Action _onFinalize;

        public readonly Army Army;

        public readonly Hero LeaderHero;

        public readonly MobileParty LeaderParty;

        private readonly Hero NewLeader;

        private readonly WarfareArmyManagementItemVM _mainPartyItem;

        private readonly float _initialGold;

        private readonly float _initialInfluence;

        private string _latestTutorialElementID;

        private string _playerDoesntHaveEnoughGoldStr;

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

        private WarfareArmyManagementItemVM _focusedItem;

        private MBBindingList<WarfareArmyManagementItemVM> _partyList;

        private MBBindingList<WarfareArmyManagementItemVM> _partiesInCart;

        private MBBindingList<WarfareArmyManagementItemVM> _partiesToRemove;

        private WarfareArmyManagementSortControllerVM _sortControllerVM;

        private int _totalStrength;

        private int _totalGoldCost;

        private int _totalInfluenceCost;

        private int _cohesion;

        private int _cohesionBoostCost;

        private string _cohesionText;

        private int _newCohesion;

        private string _totalStrengthText;

        private string _totalCostText;

        private string _totalGoldCostNumbersText;

        private string _totalInfluenceCostNumbersText;

        private string _totalInfluence;

        private string _totalLords;

        private string _costText;

        private string _strengthText;

        private string _shipCountText;

        private string _lordsText;

        private string _distanceText;

        private string _clanText;

        private string _ownerText;

        private string _nameText;

        private string _disbandArmyText;

        private string _cohesionBoostAmountText;

        private bool _playerHasArmy;

        private bool _canDisbandArmy;

        private bool _canAffordGoldCost;

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

        private bool _isSplitArmy;

        public WarfareArmyManagementVM(Action onClose, Action onFinalize = null!, Hero newLeader = null!)
        {
            if (onFinalize == null)
            {
                Army = MobileParty.MainParty.Army;
                LeaderParty = MobileParty.MainParty;
                LeaderHero = Hero.MainHero;

            }
            else
            {
                Army = VMHelper.Military!.CurrentSelectedArmy.Army;
                LeaderParty = Army.LeaderParty;
                LeaderHero = LeaderParty.LeaderHero;
            }
            NewLeader = newLeader;
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
            CanAffordGoldCost = true;
            CanAffordInfluenceCost = true;
            IsSplitArmy = newLeader != null;
            PlayerHasArmy = !IsSplitArmy && Army != null;
            if (!IsSplitArmy)
            {
                foreach (MobileParty item in MobileParty.All)
                {
                    if (item.LeaderHero != null && item.MapFaction == Hero.MainHero.MapFaction && item.LeaderHero != LeaderHero && !item.IsCaravan)
                    {
                        PartyList.Add(new WarfareArmyManagementItemVM(OnAddToCart, OnRemove, OnFocus, item));
                    }
                }
                _mainPartyItem = new WarfareArmyManagementItemVM(null, null, null, LeaderParty)
                {
                    IsTransferDisabled = true,
                    IsAlreadyWithPlayer = true,
                    IsMainHero = true,
                    IsInCart = true
                };
                PartiesInCart.Add(_mainPartyItem);
                foreach (WarfareArmyManagementItemVM party in PartyList)
                {
                    if (Army != null && party.Party.Army == Army && party.Party != LeaderParty)
                    {
                        party.Cost = 0;
                        party.IsAlreadyWithPlayer = true;
                        party.IsInCart = true;
                        PartiesInCart.Add(party);
                    }
                }
            }
            else
            {
                foreach (MobileParty item in Army!.Parties)
                {
                    if (item.LeaderHero != Hero.MainHero && item.LeaderHero != newLeader && item.LeaderHero != Army.LeaderParty.LeaderHero)
                    {
                        PartyList.Add(new WarfareArmyManagementItemVM(OnAddToCart, OnRemove, OnFocus, item, newLeader));
                    }
                }
            }
            CalculateCohesion();
            CanBoostCohesion = PlayerHasArmy && NewCohesion < 100;
            if (Army != null)
            {
                CohesionBoostCost = Campaign.Current.Models.ArmyManagementCalculationModel.GetCohesionBoostInfluenceCost(Army, 10);
            }
            _initialGold = Hero.MainHero.Gold;
            _initialInfluence = Hero.MainHero.Clan.Influence;
            OnRefresh();
            Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
            SortControllerVM = new WarfareArmyManagementSortControllerVM(ref _partyList);
            Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(OnTutorialNotificationElementIDChange);
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            if (!IsSplitArmy)
            {
                TitleText = GameTexts.FindText("str_army_management").ToString();
            }
            else
            {
                TitleText = new TextObject("{=8MvD4u4J}Split Army").ToString();
            }
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
            ShipCountText = new TextObject("{=7Q8ufo5X}Ships", null).ToString();
            DisbandCost = Army != null && (Hero.MainHero == Hero.MainHero.MapFaction.Leader || Army.LeaderParty == MobileParty.MainParty) ? 0 : Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfDisbandingArmy();
            CanDisbandArmy = GetCanDisbandArmyWithReason(out var disabledReason);
            DisbandArmyHint.HintText = disabledReason;
            _playerDoesntHaveEnoughGoldStr = GameTexts.FindText("str_warning_you_dont_have_enough_influence").ToString();
            _playerDoesntHaveEnoughInfluenceStr = GameTexts.FindText("str_warning_you_dont_have_enough_influence").ToString();
            GameTexts.SetVariable("TOTAL_INFLUENCE", MathF.Round(Hero.MainHero.Clan.Influence));
            TotalInfluence = GameTexts.FindText("str_total_influence").ToString();
            GameTexts.SetVariable("NUMBER", 10);
            CohesionBoostAmountText = GameTexts.FindText("str_plus_with_number").ToString();
            PartyList.ApplyActionOnAllItems(delegate (WarfareArmyManagementItemVM x)
            {
                x.RefreshValues();
            });
            PartiesInCart.ApplyActionOnAllItems(delegate (WarfareArmyManagementItemVM x)
            {
                x.RefreshValues();
            });
            TutorialNotification.RefreshValues();
        }

        public void OnFrameTick(GauntletLayer gauntlet)
        {
            if (gauntlet.Input.IsHotKeyPressed("Exit"))
            {
                ExecuteCancel();
            }
            else if (gauntlet.Input.IsHotKeyPressed("Confirm"))
            {
                ExecuteDone();
            }
            else if (gauntlet.Input.IsHotKeyPressed("Reset"))
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
            foreach (WarfareArmyManagementItemVM item in PartiesInCart)
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

        private void OnFocus(WarfareArmyManagementItemVM focusedItem)
        {
            FocusedItem = focusedItem;
        }

        private void OnAddToCart(WarfareArmyManagementItemVM armyItem)
        {
            if (!PartiesInCart.Contains(armyItem))
            {
                PartiesInCart.Add(armyItem);
                armyItem.IsInCart = true;
                if (!IsSplitArmy)
                {
                    Game.Current.EventManager.TriggerEvent(new PartyAddedToArmyByPlayerEvent(armyItem.Party));
                    if (_partiesToRemove.Contains(armyItem))
                    {
                        _partiesToRemove.Remove(armyItem);
                    }

                    if (armyItem.IsAlreadyWithPlayer)
                    {
                        armyItem.CanJoinBackWithoutCost = false;
                    }
                }
                if (Settings.Current.CallMercenaryToArmyCostType.SelectedIndex == 0 && armyItem.Clan.IsMinorFaction)
                {
                    TotalGoldCost += armyItem.Cost;
                }
                else
                {
                    TotalInfluenceCost += armyItem.Cost;
                }
            }

            CalculateCohesion();
            OnRefresh();
        }

        private void OnRemove(WarfareArmyManagementItemVM armyItem)
        {
            if (PartiesInCart.Contains(armyItem))
            {
                PartiesInCart.Remove(armyItem);
                armyItem.IsInCart = false;
                if (!IsSplitArmy)
                {
                    _partiesToRemove.Add(armyItem);
                    if (armyItem.IsAlreadyWithPlayer)
                    {
                        armyItem.CanJoinBackWithoutCost = true;
                    }
                }
                if (Settings.Current.CallMercenaryToArmyCostType.SelectedIndex == 0 && armyItem.Clan.IsMinorFaction)
                {
                    TotalGoldCost -= armyItem.Cost;
                }
                else
                {
                    TotalInfluenceCost -= armyItem.Cost;
                }
            }

            CalculateCohesion();
            OnRefresh();
        }

        private void ApplyCohesionChange()
        {
            if (Army == null)
            {
                return;
            }
            int num = NewCohesion - Cohesion;
            if (_influenceSpentForCohesionBoosting > 0 || Army.Parties.All((MobileParty p) => p.ActualClan == Army.LeaderParty.ActualClan))
            {
                Army.BoostCohesionWithInfluence(num, _influenceSpentForCohesionBoosting);
            }
        }

        private void OnBoostCohesion()
        {
            if (Army == null || Army.Cohesion >= 100f)
            {
                return;
            }
            if (Hero.MainHero.Clan.Influence >= (float)(CohesionBoostCost + TotalInfluenceCost))
            {
                NewCohesion += 10;
                TotalInfluenceCost += CohesionBoostCost;
                _boostedCohesion += 10;
                _influenceSpentForCohesionBoosting += CohesionBoostCost;
                OnRefresh();
            }
            else
            {
                MBInformationManager.AddQuickInformation(new TextObject("{=Xmw93W6a}Not Enough Influence"));
            }
        }

        private void OnRefresh()
        {
            TotalStrength = (from x in PartiesInCart select MathF.Round(x.Party.Party.EstimatedStrength)).Sum();
            GameTexts.SetVariable("LEFT", GameTexts.FindText("str_total_cost").ToString());
            TotalCostText = GameTexts.FindText("str_LEFT_colon").ToString();
            GameTexts.SetVariable("LEFT", CampaignUIHelper.GetAbbreviatedValueTextFromValue(TotalGoldCost));
            GameTexts.SetVariable("RIGHT", CampaignUIHelper.GetAbbreviatedValueTextFromValue(Hero.MainHero.Gold));
            TotalGoldCostNumbersText = GameTexts.FindText("str_LEFT_over_RIGHT").ToString();
            GameTexts.SetVariable("LEFT", TotalInfluenceCost.ToString());
            GameTexts.SetVariable("RIGHT", ((int)Hero.MainHero.Clan.Influence).ToString());
            TotalInfluenceCostNumbersText = GameTexts.FindText("str_LEFT_over_RIGHT").ToString();
            GameTexts.SetVariable("NUM", PartiesInCart.Count());
            TotalLords = GameTexts.FindText("str_NUM_lords").ToString();
            GameTexts.SetVariable("LEFT", GameTexts.FindText("str_strength").ToString());
            TotalStrengthText = GameTexts.FindText("str_LEFT_colon").ToString();
            CanCreateArmy = TotalGoldCost <= Hero.MainHero.Gold && (float)TotalInfluenceCost <= Hero.MainHero.Clan.Influence && PartiesInCart.Count() > 1;
            PlayerHasArmy = !IsSplitArmy && Army != null && (_partiesToRemove.Count <= 0 || PartiesInCart.Count((WarfareArmyManagementItemVM p) => p.IsAlreadyWithPlayer) >= 1);
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
            PartiesInCart.Sort(new ManagementItemComparer());
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

            if (LeaderParty.MapEvent != null)
            {
                disabledReason = new TextObject("{=uipNpzVw}Cannot disband the army right now.");
                return false;
            }
            
            if (LeaderParty.SiegeEvent != null)
            {
                disabledReason = new TextObject("{=mGWrSJLE}You cannot manage an army while it's in a map event.");
                return false;
            }

            if (!CampaignUIHelper.GetMapScreenActionIsEnabledWithReason(out var disabledReason2))
            {
                disabledReason = disabledReason2;
                return false;
            }

            disabledReason = TextObject.GetEmpty();
            return true;
        }

        private void UpdateTooltips()
        {
            if (PlayerHasArmy)
            {
                CohesionHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetArmyCohesionTooltip(Army));
                Army.RecalculateArmyMorale();
                MathF.Round(Army.Morale, 1).ToString("0.0");
                MBTextManager.SetTextVariable("BASE_EFFECT", MathF.Round(LeaderParty.Morale, 1).ToString("0.0"));
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
                GameTexts.SetVariable("reg1", (int)LeaderParty.Morale);
                MoraleHint.HintText = GameTexts.FindText("str_morale_reg1");
            }

            DoneHint.HintText = new TextObject("{=!}" + (!CanAffordInfluenceCost ? _playerDoesntHaveEnoughInfluenceStr : CanAffordGoldCost ? _playerDoesntHaveEnoughGoldStr : null));
            MBTextManager.SetTextVariable("newline", "\n");
            MBTextManager.SetTextVariable("DAILY_FOOD_CONSUMPTION", LeaderParty.FoodChange);
            FoodHint.HintText = GameTexts.FindText("str_food_consumption_tooltip");
        }

        public void ExecuteDone()
        {
            if (!CanAffordGoldCost)
            {
                return;
            }
            if (!CanAffordInfluenceCost)
            {
                return;
            }
            if (NewCohesion > Cohesion)
            {
                ApplyCohesionChange();
            }
            if (!IsSplitArmy)
            {
                if (PartiesInCart.Count > 1 && MobileParty.MainParty.MapFaction.IsKingdomFaction)
                {
                    if (Army == null)
                    {
                        ((Kingdom)MobileParty.MainParty.MapFaction).CreateArmy(Hero.MainHero, Hero.MainHero.HomeSettlement, Army.ArmyTypes.Patrolling);
                    }

                    foreach (WarfareArmyManagementItemVM item in PartiesInCart)
                    {
                        if (item.Party != LeaderParty)
                        {
                            item.Party.Army = LeaderParty.Army;
                            SetPartyAiAction.GetActionForEscortingParty(item.Party, LeaderParty, MobileParty.NavigationType.Default, false, false);
                        }
                    }
                    GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, -TotalGoldCost);
                    ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -(TotalInfluenceCost - _influenceSpentForCohesionBoosting));
                }

                if (_partiesToRemove.Count > 0)
                {
                    bool flag = false;
                    foreach (WarfareArmyManagementItemVM item2 in _partiesToRemove)
                    {
                        if (item2.Party == LeaderParty)
                        {
                            item2.Party.Army = null;
                            flag = true;
                        }
                    }

                    if (!flag)
                    {
                        foreach (WarfareArmyManagementItemVM item3 in _partiesToRemove)
                        {
                            if (Army?.Parties.Contains(item3.Party) ?? false)
                            {
                                item3.Party.Army = null;
                            }
                        }
                    }

                    _partiesToRemove.Clear();
                }
                if (_onFinalize != null)
                {
                    _onFinalize();
                }
            }
            else
            {
                if (PartiesInCart.Count > 0)
                {
                    GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, -TotalGoldCost);
                    ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -TotalInfluenceCost);
                    if (LeaderHero != Hero.MainHero)
                    {
                        GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, TotalGoldCost / 2);
                        ChangeClanInfluenceAction.Apply(LeaderParty.ActualClan, TotalInfluenceCost / 2);
                    }
                    NewLeader.Clan.Kingdom.CreateArmy(NewLeader, (from p in PartiesInCart select p.Party).ToList());
                    if (_onFinalize != null)
                    {
                        _onFinalize();
                    }
                }
            }
            _onClose();
            CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
        }

        public void ExecuteCancel()
        {
            if (!IsSplitArmy)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, (int)(_initialGold - Hero.MainHero.Gold));
                ChangeClanInfluenceAction.Apply(Clan.PlayerClan, _initialInfluence - Clan.PlayerClan.Influence);
            }
            _onClose();
        }

        public void ExecuteReset()
        {
            foreach (WarfareArmyManagementItemVM item in PartiesInCart.ToList())
            {
                OnRemove(item);
                item.UpdateEligibility();
            }
            if (!IsSplitArmy)
            {
                PartiesInCart.Add(_mainPartyItem);
                foreach (WarfareArmyManagementItemVM party in PartyList)
                {
                    if (party.IsAlreadyWithPlayer)
                    {
                        PartiesInCart.Add(party);
                        party.IsInCart = true;
                        party.CanJoinBackWithoutCost = false;
                    }
                }

                NewCohesion = Cohesion;
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, (int)(_initialGold - Hero.MainHero.Gold));
                ChangeClanInfluenceAction.Apply(Clan.PlayerClan, _initialInfluence - Clan.PlayerClan.Influence);
                _boostedCohesion = 0;
                _influenceSpentForCohesionBoosting = 0;
                _partiesToRemove.Clear();
            }
            TotalGoldCost = 0;
            TotalInfluenceCost = 0;
            OnRefresh();
        }

        public void ExecuteDisbandArmy()
        {
            if (CanDisbandArmy)
            {
                TextObject inquiryText = new TextObject("{=619gtcXb}Are you sure you want to disband this army? This will result in relation loss.");
                if (LeaderParty == MobileParty.MainParty)
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
            foreach (WarfareArmyManagementItemVM item in PartiesInCart.ToList())
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
            if (_onFinalize != null)
            {
                _onFinalize();
            }
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
            get => _tutorialNotification;
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
        public WarfareArmyManagementSortControllerVM SortControllerVM
        {
            get => _sortControllerVM;
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
            get => _boostTitleText;
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
            get => _disbandArmyText;
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
            get => _cohesionBoostAmountText;
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
            get => _distanceText;
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
            get => _costText;
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
            get => _ownerText;
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
            get => _strengthText;
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
            get => _lordsText;
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
        public string ShipCountText
        {
            get => _shipCountText;
            set
            {
                if (value != _shipCountText)
                {
                    _shipCountText = value;
                    OnPropertyChangedWithValue(value, "ShipCountText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalInfluence
        {
            get => _totalInfluence;
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
            get => _totalStrength;
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
        public int TotalGoldCost
        {
            get => _totalGoldCost;
            set
            {
                if (value != _totalGoldCost)
                {
                    _totalGoldCost = value;
                    CanAffordGoldCost = TotalGoldCost <= 0 || TotalGoldCost <= Hero.MainHero.Gold;
                    OnPropertyChangedWithValue(value, "TotalGoldCost");
                }
            }
        }

        [DataSourceProperty]
        public int TotalInfluenceCost
        {
            get => _totalInfluenceCost;
            set
            {
                if (value != _totalInfluenceCost)
                {
                    _totalInfluenceCost = value;
                    CanAffordInfluenceCost = TotalInfluenceCost <= 0 || (float)TotalInfluenceCost <= Hero.MainHero.Clan.Influence;
                    OnPropertyChangedWithValue(value, "TotalInfluenceCost");
                }
            }
        }

        [DataSourceProperty]
        public string TotalLords
        {
            get => _totalLords;
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
            get => _canCreateArmy;
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
            get => _canBoostCohesion;
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
            get => _canDisbandArmy;
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
        public bool CanAffordGoldCost
        {
            get => _canAffordGoldCost;
            set
            {
                if (value != _canAffordGoldCost)
                {
                    _canAffordGoldCost = value;
                    OnPropertyChangedWithValue(value, "CanAffordGoldCost");
                }
            }
        }

        [DataSourceProperty]
        public bool CanAffordInfluenceCost
        {
            get => _canAffordInfluenceCost;
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
            get => _titleText;
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
            get => _clanText;
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
            get => _nameText;
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
            get => _cancelText;
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
            get => _doneText;
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
        public WarfareArmyManagementItemVM FocusedItem
        {
            get => _focusedItem;
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
        public MBBindingList<WarfareArmyManagementItemVM> PartyList
        {
            get => _partyList;
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
        public MBBindingList<WarfareArmyManagementItemVM> PartiesInCart
        {
            get => _partiesInCart;
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
            get => _totalStrengthText;
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
            get => _totalCostText;
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
        public string TotalGoldCostNumbersText
        {
            get => _totalGoldCostNumbersText;
            set
            {
                if (value != _totalGoldCostNumbersText)
                {
                    _totalGoldCostNumbersText = value;
                    OnPropertyChangedWithValue(value, "TotalGoldCostNumbersText");
                }
            }
        }

        [DataSourceProperty]
        public string TotalInfluenceCostNumbersText
        {
            get => _totalInfluenceCostNumbersText;
            set
            {
                if (value != _totalInfluenceCostNumbersText)
                {
                    _totalInfluenceCostNumbersText = value;
                    OnPropertyChangedWithValue(value, "TotalInfluenceCostNumbersText");
                }
            }
        }

        [DataSourceProperty]
        public string CohesionText
        {
            get => _cohesionText;
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
            get => _cohesion;
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
            get => _cohesionBoostCost;
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
            get => _playerHasArmy;
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
            get => _moraleText;
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
            get => _foodText;
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
            get => _newCohesion;
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
            get => _cohesionHint;
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
            get => _moraleHint;
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
            get => _boostCohesionHint;
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
            get => _disbandArmyHint;
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
            get => _doneHint;
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
            get => _foodHint;
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
            get => _disbandCost;
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
            get => _resetInputKey;
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
            get => _cancelInputKey;
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
            get => _doneInputKey;
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
            get => _removeInputKey;
            set
            {
                if (value == _removeInputKey)
                {
                    return;
                }

                _removeInputKey = value;
                OnPropertyChangedWithValue(value, "RemoveInputKey");
                foreach (WarfareArmyManagementItemVM party in PartyList)
                {
                    party.RemoveInputKey = value;
                }
            }
        }

        [DataSourceProperty]
        public bool IsDisbandWithCost
        {
            get => LeaderParty == null || LeaderParty == MobileParty.MainParty;
        }

        [DataSourceProperty]
        public bool IsSplitArmy
        {
            get => _isSplitArmy;
            set
            {
                if (value != _isSplitArmy)
                {
                    _isSplitArmy = value;
                    OnPropertyChangedWithValue(value, "IsSplitArmy");
                }
            }
        }
    }
}