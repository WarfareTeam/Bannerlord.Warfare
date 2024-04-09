using System;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using Warfare.Extensions;

namespace Warfare.ViewModels.ArmyManagement
{
    internal class SplitArmyVM : ViewModel
    {
        private readonly Army Army;
        private readonly Hero NewLeader;
        private readonly Action _onClose;
        private readonly Action _onFinalize;

        private string _latestTutorialElementID;
        private SplitArmyItemVM _focusedItem;
        private MBBindingList<SplitArmyItemVM> _partyList;
        private MBBindingList<SplitArmyItemVM> _partiesInCart;
        private SplitArmySortControllerVM _sortControllerVM;

        private bool _canAffordInfluenceCost;
        private int _totalStrength;
        private int _totalCost;
        private string _totalCostText;

        private HintViewModel _doneHint;

        public ElementNotificationVM _tutorialNotification;

        private InputKeyItemVM _resetInputKey;
        private InputKeyItemVM _cancelInputKey;
        private InputKeyItemVM _doneInputKey;
        private InputKeyItemVM _removeInputKey;


        public SplitArmyVM(Army army, Hero newLeader, Action onClose, Action onFinalize)
        {
            Army = army;
            NewLeader = newLeader;
            _onClose = onClose;
            _onFinalize = onFinalize;
            PartyList = new MBBindingList<SplitArmyItemVM>();
            PartiesInCart = new MBBindingList<SplitArmyItemVM>();
            DoneHint = new HintViewModel();
            TutorialNotification = new ElementNotificationVM();
            CanAffordInfluenceCost = true;
            foreach (MobileParty party in army.Parties)
            {
                if (party.LeaderHero != Hero.MainHero && party.LeaderHero != newLeader && party.LeaderHero != army.LeaderParty.LeaderHero)
                {
                    PartyList.Add(new SplitArmyItemVM(OnAddToCart, OnRemoveFromCart, OnFocus, newLeader, party));
                }
            }
            Game.Current.EventManager.TriggerEvent(new TutorialContextChangedEvent(TutorialContexts.ArmyManagement));
            SortControllerVM = new SplitArmySortControllerVM(ref _partyList);
            Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(OnTutorialNotificationElementIDChange);
            OnRefresh();
        }

        private void OnRefresh()
        {
            TotalStrength = (from x in PartiesInCart select Campaign.Current.Models.ArmyManagementCalculationModel.GetPartyStrength(x.Party.Party)).Sum();
            TotalCostText = TotalCost + " / " + (int)Clan.PlayerClan.Influence;
            DoneHint.HintText = new TextObject("{=!}" + (CanAffordInfluenceCost ? null : GameTexts.FindText("str_warning_you_dont_have_enough_influence").ToString()));
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            PartyList.ApplyActionOnAllItems(delegate (SplitArmyItemVM x)
            {
                x.RefreshValues();
            });
            PartiesInCart.ApplyActionOnAllItems(delegate (SplitArmyItemVM x)
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

        private void OnFocus(SplitArmyItemVM focusedItem)
        {
            FocusedItem = focusedItem;
        }

        private void OnAddToCart(SplitArmyItemVM armyItem)
        {
            if (!PartiesInCart.Contains(armyItem))
            {
                PartiesInCart.Add(armyItem);
                armyItem.IsInCart = true;
                TotalCost += armyItem.Cost;
            }
            OnRefresh();
        }

        private void OnRemoveFromCart(SplitArmyItemVM armyItem)
        {
            if (PartiesInCart.Contains(armyItem))
            {
                PartiesInCart.Remove(armyItem);
                armyItem.IsInCart = false;
                TotalCost -= armyItem.Cost;
            }
            OnRefresh();
        }

        public void ExecuteDone()
        {
            if (!CanAffordInfluenceCost)
            {
                return;
            }
            if (PartiesInCart.Count > 0)
            {
                ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -TotalCost);
                if (Army.LeaderParty.LeaderHero != Hero.MainHero)
                {
                    ChangeClanInfluenceAction.Apply(Army.LeaderParty.ActualClan, TotalCost / 2);
                };
                NewLeader.Clan.Kingdom.CreateArmy(NewLeader, (from p in PartiesInCart select p.Party).ToList());
                _onFinalize();
            }
            _onClose();
        }
        public void ExecuteCancel()
        {
            _onClose();
        }

        public void ExecuteReset()
        {
            foreach (SplitArmyItemVM item in PartiesInCart.ToList())
            {
                OnRemoveFromCart(item);
            }
            TotalCost = 0;
            OnRefresh();
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
            CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
        }

        public void SetDoneInputKey(HotKey hotKey)
        {
            DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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
        public SplitArmySortControllerVM SortControllerVM
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
        public SplitArmyItemVM FocusedItem
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
        public MBBindingList<SplitArmyItemVM> PartyList
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
        public MBBindingList<SplitArmyItemVM> PartiesInCart
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
                    CanAffordInfluenceCost = TotalCost <= 0 || TotalCost <= Hero.MainHero.Clan.Influence;
                    OnPropertyChangedWithValue(value, "TotalCost");
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
                foreach (SplitArmyItemVM party in PartyList)
                {
                    party.RemoveInputKey = value;
                }
            }
        }
    }
}
