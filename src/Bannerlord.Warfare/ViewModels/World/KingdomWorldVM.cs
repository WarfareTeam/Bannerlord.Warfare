using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace Warfare.ViewModels.World
{
    public class KingdomWorldVM : KingdomCategoryVM
    {
        public class WorldWarNameComparer : IComparer<WorldWarItemVM>
        {
            public int Compare(WorldWarItemVM x, WorldWarItemVM y)
            {
                return x.Faction1Name.CompareTo(y.Faction1Name);
            }
        }
        private bool _isChangingDiplomacyItem;
        private MBBindingList<WorldWarItemVM> _wars;
        private WorldDiplomacyItemVM _currentSelectedItem;
        private SelectorVM<SelectorItemVM> _behaviorSelection;
        private HintViewModel _showStatBarsHint;
        private HintViewModel _showWarLogsHint;
        private HintViewModel _actionHint;
        private string _warsText;
        private string _numOfWarsText;
        private string _diplomacyText;
        private bool _isDisplayingWarLogs;
        private bool _isDisplayingStatComparisons;
        private bool _isWar;
        public KingdomWorldVM()
        {
            Wars = new MBBindingList<WorldWarItemVM>();
            ActionHint = new HintViewModel();
            ExecuteShowStatComparisons();
            RefreshValues();
            SetDefaultSelectedItem();
        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            RefreshWorldList();
            NoItemSelectedText = GameTexts.FindText("str_kingdom_no_war_selected", null).ToString();
            DiplomacyText = GameTexts.FindText("str_diplomatic_group", null).ToString();
            WarsText = new TextObject("{=!!}External Wars").ToString();
            ShowStatBarsHint = new HintViewModel(GameTexts.FindText("str_kingdom_war_show_comparison_bars", null), null);
            ShowWarLogsHint = new HintViewModel(GameTexts.FindText("str_kingdom_war_show_war_logs", null), null);
            Wars.ApplyActionOnAllItems(delegate (WorldWarItemVM x)
            {
                x.RefreshValues();
            });
            WorldDiplomacyItemVM currentSelectedDiplomacyItem = CurrentSelectedDiplomacyItem;
            if (currentSelectedDiplomacyItem == null)
            {
                return;
            }
            currentSelectedDiplomacyItem.RefreshValues();
        }
        public void RefreshWorldList()
        {
            Wars.Clear();
            IEnumerable<Kingdom> kingdoms = Kingdom.All.WhereQ(k => Hero.MainHero.MapFaction as Kingdom != k);
            foreach (Kingdom kingdom in kingdoms)
            {
                Kingdom playerKingdom = Hero.MainHero.MapFaction as Kingdom;
                if (kingdom != playerKingdom)
                {
                    foreach (Kingdom kingdom2 in kingdoms)
                    {
                        if (kingdom != playerKingdom && kingdom != kingdom2 && !ContainsWar(kingdom, kingdom2))
                        {
                            if (FactionManager.IsAtWarAgainstFaction(kingdom, kingdom2))
                            {
                                Wars.Add(new WorldWarItemVM(kingdom.GetStanceWith(kingdom2), OnDiplomacyItemSelection));
                            }
                        }
                    }
                }
            }
            GameTexts.SetVariable("STR", Wars.Count);
            NumOfWarsText = GameTexts.FindText("str_STR_in_parentheses", null).ToString();
            Wars.Sort(new WorldWarNameComparer());
            SetDefaultSelectedItem();
        }

        public bool ContainsWar(IFaction faction, IFaction faction2)
        {
            foreach (WorldWarItemVM war in Wars)
            {
                if ((war.Faction1 == faction || war.Faction1 == faction2) && (war.Faction2 == faction || war.Faction2 == faction2))
                {
                    return true;
                }
            }
            return false;
        }
        public void OnSetCurrentDiplomacyItem(WorldDiplomacyItemVM item)
        {
            RefreshCurrentWarVisuals(item);
        }
        private void RefreshCurrentWarVisuals(WorldDiplomacyItemVM item)
        {
            if (item != null)
            {
                if (CurrentSelectedDiplomacyItem != null)
                {
                    CurrentSelectedDiplomacyItem.IsSelected = false;
                }
                CurrentSelectedDiplomacyItem = item;
                if (CurrentSelectedDiplomacyItem != null)
                {
                    CurrentSelectedDiplomacyItem.IsSelected = true;
                }
            }
        }
        private void OnDiplomacyItemSelection(WorldDiplomacyItemVM item)
        {
            if (CurrentSelectedDiplomacyItem != item)
            {
                if (CurrentSelectedDiplomacyItem != null)
                {
                    CurrentSelectedDiplomacyItem.IsSelected = false;
                }
                CurrentSelectedDiplomacyItem = item;
                IsAcceptableItemSelected = item != null;
                OnSetCurrentDiplomacyItem(item);
            }
        }
        private void ExecuteShowWarLogs()
        {
            IsDisplayingWarLogs = true;
            IsDisplayingStatComparisons = false;
        }
        private void ExecuteShowStatComparisons()
        {
            IsDisplayingWarLogs = false;
            IsDisplayingStatComparisons = true;
        }
        private void SetDefaultSelectedItem()
        {
            WorldDiplomacyItemVM warItem = Wars.FirstOrDefault();
            OnDiplomacyItemSelection(warItem);
        }
        private void OnBehaviorSelectionChanged(SelectorVM<SelectorItemVM> s)
        {
            if (!_isChangingDiplomacyItem && CurrentSelectedDiplomacyItem != null)
            {
                CurrentSelectedDiplomacyItem.Faction1.GetStanceWith(CurrentSelectedDiplomacyItem.Faction2).BehaviorPriority = s.SelectedIndex;
            }
        }
        [DataSourceProperty]
        public MBBindingList<WorldWarItemVM> Wars
        {
            get => _wars;
            set
            {
                if (value != _wars)
                {
                    _wars = value;
                    OnPropertyChangedWithValue(value, "Wars");
                }
            }
        }
        [DataSourceProperty]
        public bool IsDisplayingWarLogs
        {
            get => _isDisplayingWarLogs;
            set
            {
                if (value != _isDisplayingWarLogs)
                {
                    _isDisplayingWarLogs = value;
                    OnPropertyChangedWithValue(value, "IsDisplayingWarLogs");
                }
            }
        }
        [DataSourceProperty]
        public bool IsDisplayingStatComparisons
        {
            get =>_isDisplayingStatComparisons;
            set
            {
                if (value != _isDisplayingStatComparisons)
                {
                    _isDisplayingStatComparisons = value;
                    OnPropertyChangedWithValue(value, "IsDisplayingStatComparisons");
                }
            }
        }
        [DataSourceProperty]
        public bool IsWar
        {
            get => _isWar;
            set
            {
                if (value != _isWar)
                {
                    _isWar = value;
                    if (!value)
                    {
                        ExecuteShowStatComparisons();
                    }
                    OnPropertyChangedWithValue(value, "IsWar");
                }
            }
        }
        [DataSourceProperty]
        public WorldDiplomacyItemVM CurrentSelectedDiplomacyItem
        {
            get => _currentSelectedItem;
            set
            {
                if (value != _currentSelectedItem)
                {
                    _isChangingDiplomacyItem = true;
                    _currentSelectedItem = value;
                    IsWar = value is WorldWarItemVM;
                    OnPropertyChangedWithValue(value, "CurrentSelectedDiplomacyItem");
                    _isChangingDiplomacyItem = false;
                }
            }
        }
        [DataSourceProperty]
        public string DiplomacyText
        {
            get => _diplomacyText;
            set
            {
                if (value != _diplomacyText)
                {
                    _diplomacyText = value;
                    OnPropertyChangedWithValue(value, "DiplomacyText");
                }
            }
        }
        [DataSourceProperty]
        public string WarsText
        {
            get => _warsText;
            set
            {
                if (value != _warsText)
                {
                    _warsText = value;
                    OnPropertyChangedWithValue(value, "WarsText");
                }
            }
        }
        [DataSourceProperty]
        public string NumOfWarsText
        {
            get => _numOfWarsText;
            set
            {
                if (value != _numOfWarsText)
                {
                    _numOfWarsText = value;
                    OnPropertyChangedWithValue(value, "NumOfWarsText");
                }
            }
        }
        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> BehaviorSelection
        {
            get => _behaviorSelection;
            set
            {
                if (value != _behaviorSelection)
                {
                    _behaviorSelection = value;
                    OnPropertyChangedWithValue(value, "BehaviorSelection");
                }
            }
        }
        [DataSourceProperty]
        public HintViewModel ShowStatBarsHint
        {
            get => _showStatBarsHint;
            set
            {
                if (value != _showStatBarsHint)
                {
                    _showStatBarsHint = value;
                    OnPropertyChangedWithValue(value, "ShowStatBarsHint");
                }
            }
        }
        [DataSourceProperty]
        public HintViewModel ShowWarLogsHint
        {
            get => _showWarLogsHint;
            set
            {
                if (value != _showWarLogsHint)
                {
                    _showWarLogsHint = value;
                    OnPropertyChangedWithValue(value, "ShowWarLogsHint");
                }
            }
        }
        [DataSourceProperty]
        public HintViewModel ActionHint
        {
            get => _actionHint;
            set
            {
                if (value != _actionHint)
                {
                    _actionHint = value;
                    OnPropertyChangedWithValue(value, "ActionHint");
                }
            }
        }
    }
}
