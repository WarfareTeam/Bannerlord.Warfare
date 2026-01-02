using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;

using Warfare.Extensions;
using TaleWorlds.Localization;

namespace Warfare.ViewModels.Military
{
    public class KingdomMercenaryItemVM : KingdomItemVM
    {
        private readonly Action<KingdomMercenaryItemVM> _onSelect;

        public readonly Clan Clan;

        private HeroVM _leader;

        private MBBindingList<KingdomArmyPartyItemVM> _parties;

        private string _clanName;

        private int _infantryCount;

        private int _rangedCount;

        private int _cavalryCount;

        private int _horseArcherCount;

        private int _totalManCount;

        private int _hireCost;

        private string _totalManCountLabel;

        private string _brushText;

        private string _hiredText;

        private bool _isHired;

        private bool _isMainClan;

        public KingdomMercenaryItemVM(Clan clan, Action<KingdomMercenaryItemVM> onSelect)
        {
            Clan = clan;
            _onSelect = onSelect;
            CampaignUIHelper.GetCharacterCode(clan.Leader.CharacterObject, false);
            Leader = new HeroVM(clan.Leader);
            // IsNew = true;
            ClanName = clan.Name.ToString();
            BrushText = "Warfare.Mercenary.Ally.Text.Tuple";
            HiredText = "";
            IsHired = false;
            InfantryCount = 0;
            RangedCount = 0;
            CavalryCount = 0;
            HorseArcherCount = 0;
            foreach (WarPartyComponent component in Clan.WarPartyComponents)
            {
                foreach (TroopRosterElement item in component.MobileParty.MemberRoster.GetTroopRoster())
                {
                    if (item.Character.DefaultFormationClass.Equals(FormationClass.Infantry))
                    {
                        InfantryCount += item.Number;
                    }
                    else if (item.Character.DefaultFormationClass.Equals(FormationClass.Ranged))
                    {
                        RangedCount += item.Number;
                    }
                    else if (item.Character.DefaultFormationClass.Equals(FormationClass.Cavalry))
                    {
                        CavalryCount += item.Number;
                    }
                    else if (item.Character.DefaultFormationClass.Equals(FormationClass.HorseArcher))
                    {
                        HorseArcherCount += item.Number;
                    }
                }
            }
            TotalManCount = Clan.GetRosterSize();
            GameTexts.SetVariable("LEFT", GameTexts.FindText("str_men_count"));
            GameTexts.SetVariable("RIGHT", TotalManCount);
            TotalManCountLabel = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon").ToString();
            HireCost = Clan.GetMercenaryWage();
            Parties = new MBBindingList<KingdomArmyPartyItemVM>();
            foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
            {
                if (warPartyComponent.Leader != null)
                {
                    Parties.Add(new KingdomArmyPartyItemVM(warPartyComponent.MobileParty));
                }
            }
            IsMainClan = clan == Clan.PlayerClan;
            RefreshValues();

        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            IsHired = Clan.IsUnderMercenaryService;
            if (IsHired)
            {
                HiredText = Clan == Clan.PlayerClan ? new TextObject("{=wpRXZsLv}You are under contract").ToString() : Clan.MapFaction != Hero.MainHero.MapFaction ? new TextObject("{=8SYPcFKw}Under contract").ToString() : !Hero.MainHero.IsFactionLeader ? new TextObject("{=xZbXWkys}Hired by your faction").ToString() : new TextObject("{=dRs1KZ3P}Hired by you").ToString();
                BrushText = Clan.MapFaction != Hero.MainHero.MapFaction ? "Warfare.Mercenary.Enemy.Text.Tuple" : "Warfare.Mercenary.Ally.Text.Tuple";
            }
            Parties.ApplyActionOnAllItems(delegate (KingdomArmyPartyItemVM k)
            {
                k.RefreshValues();
            });
        }
        protected override void OnSelect()
        {
            base.OnSelect();
            _onSelect(this);
        }
        // protected void ExecuteLink(string link)
        // {
        //     Campaign.Current.EncyclopediaManager.GoToLink(link);
        // }

        [DataSourceProperty]
        public HeroVM Leader
        {
            get => _leader;
            set
            {
                if (value != _leader)
                {
                    _leader = value;
                    OnPropertyChanged("Visual");
                }
            }
        }
        [DataSourceProperty]
        public string ClanName
        {
            get => _clanName;
            set
            {
                if (value != _clanName)
                {
                    _clanName = value;
                    OnPropertyChangedWithValue(value, "ClanName");
                }
            }
        }
        [DataSourceProperty]
        public int InfantryCount
        {
            get => _infantryCount;
            set
            {
                if (value != _infantryCount)
                {
                    _infantryCount = value;
                    OnPropertyChangedWithValue(value, "InfantryCount");
                }
            }
        }
        [DataSourceProperty]
        public int RangedCount
        {
            get => _rangedCount;
            set
            {
                if (value != _rangedCount)
                {
                    _rangedCount = value;
                    OnPropertyChangedWithValue(value, "RangedCount");
                }
            }
        }
        [DataSourceProperty]
        public int CavalryCount
        {
            get => _cavalryCount;
            set
            {
                if (value != _cavalryCount)
                {
                    _cavalryCount = value;
                    OnPropertyChangedWithValue(value, "CavalryCount");
                }
            }
        }
        [DataSourceProperty]
        public int HorseArcherCount
        {
            get => _horseArcherCount;
            set
            {
                if (value != _horseArcherCount)
                {
                    _horseArcherCount = value;
                    OnPropertyChangedWithValue(value, "HorseArcherCount");
                }
            }
        }
        [DataSourceProperty]
        public int TotalManCount
        {
            get => _totalManCount;
            set
            {
                if (value != _totalManCount)
                {
                    _totalManCount = value;
                    OnPropertyChangedWithValue(value, "TotalManCount");
                }
            }
        }
        [DataSourceProperty]
        public string TotalManCountLabel
        {
            get => _totalManCountLabel;
            set
            {
                if (value != _totalManCountLabel)
                {
                    _totalManCountLabel = value;
                    OnPropertyChangedWithValue(value, "TotalManCountLabel");
                }
            }
        }
        [DataSourceProperty]
        public int HireCost
        {
            get => _hireCost;
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
        public string BrushText
        {
            get => _brushText;
            set
            {
                if (value != _brushText)
                {
                    _brushText = value;
                    OnPropertyChangedWithValue(value, "BrushText");
                }
            }
        }
        [DataSourceProperty]
        public string HiredText
        {
            get => _hiredText;
            set
            {
                _hiredText = value;
                OnPropertyChangedWithValue(value, "HiredText");
            }
        }

        [DataSourceProperty]
        public bool IsHired
        {
            get => _isHired;
            set
            {
                if (value != _isHired)
                {
                    _isHired = value;
                    OnPropertyChangedWithValue(value, "IsHired");
                }
            }
        }
        [DataSourceProperty]
        public bool IsMainClan
        {
            get => _isMainClan;
            set
            {
                if (value != _isMainClan)
                {
                    _isMainClan = value;
                    OnPropertyChangedWithValue(value, "IsMainClan");
                }
            }
        }
        [DataSourceProperty]
        public MBBindingList<KingdomArmyPartyItemVM> Parties
        {
            get => _parties;
            set
            {
                if (value != _parties)
                {
                    _parties = value;
                    OnPropertyChangedWithValue(value, "Parties");
                }
            }
        }
    }
}
