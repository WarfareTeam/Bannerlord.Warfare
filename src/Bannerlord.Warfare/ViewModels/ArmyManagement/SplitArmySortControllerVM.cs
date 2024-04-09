﻿using System.Collections.Generic;

using TaleWorlds.Library;

namespace Warfare.ViewModels.ArmyManagement
{
    public class SplitArmySortControllerVM : ViewModel
    {
        private enum SortState
        {
            Default,
            Ascending,
            Descending
        }

        public abstract class ItemComparerBase : IComparer<SplitArmyItemVM>
        {
            protected bool _isAscending;

            public void SetSortMode(bool isAscending)
            {
                _isAscending = isAscending;
            }

            public abstract int Compare(SplitArmyItemVM x, SplitArmyItemVM y);

            protected int ResolveEquality(SplitArmyItemVM x, SplitArmyItemVM y)
            {
                return x.LeaderNameText.CompareTo(y.LeaderNameText);
            }
        }

        public class ItemDistanceComparer : ItemComparerBase
        {
            public override int Compare(SplitArmyItemVM x, SplitArmyItemVM y)
            {
                int num = y.DistInTime.CompareTo(x.DistInTime);
                if (num != 0)
                {
                    return num * ((!_isAscending) ? 1 : (-1));
                }

                return ResolveEquality(x, y);
            }
        }

        public class ItemCostComparer : ItemComparerBase
        {
            public override int Compare(SplitArmyItemVM x, SplitArmyItemVM y)
            {
                int num = y.Cost.CompareTo(x.Cost);
                if (num != 0)
                {
                    return num * ((!_isAscending) ? 1 : (-1));
                }

                return ResolveEquality(x, y);
            }
        }

        public class ItemStrengthComparer : ItemComparerBase
        {
            public override int Compare(SplitArmyItemVM x, SplitArmyItemVM y)
            {
                int num = y.Strength.CompareTo(x.Strength);
                if (num != 0)
                {
                    return num * ((!_isAscending) ? 1 : (-1));
                }

                return ResolveEquality(x, y);
            }
        }

        public class ItemNameComparer : ItemComparerBase
        {
            public override int Compare(SplitArmyItemVM x, SplitArmyItemVM y)
            {
                if (_isAscending)
                {
                    return y.LeaderNameText.CompareTo(x.LeaderNameText) * -1;
                }

                return y.LeaderNameText.CompareTo(x.LeaderNameText);
            }
        }

        public class ItemClanComparer : ItemComparerBase
        {
            public override int Compare(SplitArmyItemVM x, SplitArmyItemVM y)
            {
                int num = y.Clan.Name.ToString().CompareTo(x.Clan.Name.ToString());
                if (num != 0)
                {
                    return num * ((!_isAscending) ? 1 : (-1));
                }

                return ResolveEquality(x, y);
            }
        }

        private readonly MBBindingList<SplitArmyItemVM> _listToControl;

        private readonly ItemDistanceComparer _distanceComparer;

        private readonly ItemCostComparer _costComparer;

        private readonly ItemStrengthComparer _strengthComparer;

        private readonly ItemNameComparer _nameComparer;

        private readonly ItemClanComparer _clanComparer;

        private int _distanceState;

        private int _costState;

        private int _strengthState;

        private int _nameState;

        private int _clanState;

        private bool _isNameSelected;

        private bool _isCostSelected;

        private bool _isStrengthSelected;

        private bool _isDistanceSelected;

        private bool _isClanSelected;

        public SplitArmySortControllerVM(ref MBBindingList<SplitArmyItemVM> listToControl)
        {
            _listToControl = listToControl;
            _distanceComparer = new ItemDistanceComparer();
            _costComparer = new ItemCostComparer();
            _strengthComparer = new ItemStrengthComparer();
            _nameComparer = new ItemNameComparer();
            _clanComparer = new ItemClanComparer();
        }

        public void ExecuteSortByDistance()
        {
            int distanceState = DistanceState;
            SetAllStates(SortState.Default);
            DistanceState = (distanceState + 1) % 3;
            if (DistanceState == 0)
            {
                DistanceState++;
            }

            _distanceComparer.SetSortMode(DistanceState == 1);
            _listToControl.Sort(_distanceComparer);
            IsDistanceSelected = true;
        }

        public void ExecuteSortByCost()
        {
            int costState = CostState;
            SetAllStates(SortState.Default);
            CostState = (costState + 1) % 3;
            if (CostState == 0)
            {
                CostState++;
            }

            _costComparer.SetSortMode(CostState == 1);
            _listToControl.Sort(_costComparer);
            IsCostSelected = true;
        }

        public void ExecuteSortByStrength()
        {
            int strengthState = StrengthState;
            SetAllStates(SortState.Default);
            StrengthState = (strengthState + 1) % 3;
            if (StrengthState == 0)
            {
                StrengthState++;
            }

            _strengthComparer.SetSortMode(StrengthState == 1);
            _listToControl.Sort(_strengthComparer);
            IsStrengthSelected = true;
        }

        public void ExecuteSortByName()
        {
            int nameState = NameState;
            SetAllStates(SortState.Default);
            NameState = (nameState + 1) % 3;
            if (NameState == 0)
            {
                NameState++;
            }

            _nameComparer.SetSortMode(NameState == 1);
            _listToControl.Sort(_nameComparer);
            IsNameSelected = true;
        }

        public void ExecuteSortByClan()
        {
            int clanState = ClanState;
            SetAllStates(SortState.Default);
            ClanState = (clanState + 1) % 3;
            if (ClanState == 0)
            {
                ClanState++;
            }

            _clanComparer.SetSortMode(ClanState == 1);
            _listToControl.Sort(_clanComparer);
            IsClanSelected = true;
        }

        private void SetAllStates(SortState state)
        {
            DistanceState = (int)state;
            CostState = (int)state;
            StrengthState = (int)state;
            NameState = (int)state;
            ClanState = (int)state;
            IsDistanceSelected = false;
            IsCostSelected = false;
            IsNameSelected = false;
            IsClanSelected = false;
            IsStrengthSelected = false;
        }

        [DataSourceProperty]
        public int DistanceState
        {
            get
            {
                return _distanceState;
            }
            set
            {
                if (value != _distanceState)
                {
                    _distanceState = value;
                    OnPropertyChangedWithValue(value, "DistanceState");
                }
            }
        }

        [DataSourceProperty]
        public int CostState
        {
            get
            {
                return _costState;
            }
            set
            {
                if (value != _costState)
                {
                    _costState = value;
                    OnPropertyChangedWithValue(value, "CostState");
                }
            }
        }

        [DataSourceProperty]
        public int StrengthState
        {
            get
            {
                return _strengthState;
            }
            set
            {
                if (value != _strengthState)
                {
                    _strengthState = value;
                    OnPropertyChangedWithValue(value, "StrengthState");
                }
            }
        }

        [DataSourceProperty]
        public int NameState
        {
            get
            {
                return _nameState;
            }
            set
            {
                if (value != _nameState)
                {
                    _nameState = value;
                    OnPropertyChangedWithValue(value, "NameState");
                }
            }
        }

        [DataSourceProperty]
        public int ClanState
        {
            get
            {
                return _clanState;
            }
            set
            {
                if (value != _clanState)
                {
                    _clanState = value;
                    OnPropertyChangedWithValue(value, "ClanState");
                }
            }
        }

        [DataSourceProperty]
        public bool IsNameSelected
        {
            get
            {
                return _isNameSelected;
            }
            set
            {
                if (value != _isNameSelected)
                {
                    _isNameSelected = value;
                    OnPropertyChangedWithValue(value, "IsNameSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsCostSelected
        {
            get
            {
                return _isCostSelected;
            }
            set
            {
                if (value != _isCostSelected)
                {
                    _isCostSelected = value;
                    OnPropertyChangedWithValue(value, "IsCostSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsStrengthSelected
        {
            get
            {
                return _isStrengthSelected;
            }
            set
            {
                if (value != _isStrengthSelected)
                {
                    _isStrengthSelected = value;
                    OnPropertyChangedWithValue(value, "IsStrengthSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsDistanceSelected
        {
            get
            {
                return _isDistanceSelected;
            }
            set
            {
                if (value != _isDistanceSelected)
                {
                    _isDistanceSelected = value;
                    OnPropertyChangedWithValue(value, "IsDistanceSelected");
                }
            }
        }

        [DataSourceProperty]
        public bool IsClanSelected
        {
            get
            {
                return _isClanSelected;
            }
            set
            {
                if (value != _isClanSelected)
                {
                    _isClanSelected = value;
                    OnPropertyChangedWithValue(value, "IsClanSelected");
                }
            }
        }
    }
}
