using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Warfare.ViewModels.ArmyManagement
{
    public class SplitArmyItemVM : ViewModel
    {
        private readonly Action<SplitArmyItemVM> _onAddToCart;

        private readonly Action<SplitArmyItemVM> _onRemove;

        private readonly Action<SplitArmyItemVM> _onFocus;

        public readonly MobileParty Party;

        private InputKeyItemVM _removeInputKey;

        private ImageIdentifierVM _clanBanner;

        private ImageIdentifierVM _lordFace;

        private string _nameText;

        private string _leaderNameText;

        private int _relation = -102;

        private int _strength = -1;

        private string _distanceText;

        private int _cost = -1;

        private bool _isEligible;

        private TextObject _eligibleText;

        private bool _isMainHero;

        private bool _isInCart;

        private bool _isTransferDisabled;

        private bool _isFocused;

        public float DistInTime { get; }

        public float _distance { get; }

        public Clan Clan { get; }

        public SplitArmyItemVM(Action<SplitArmyItemVM> onAddToCart, Action<SplitArmyItemVM> onRemove, Action<SplitArmyItemVM> onFocus, Hero newLeader, MobileParty mobileParty)
        {
            ArmyManagementCalculationModel armyManagementCalculationModel = Campaign.Current.Models.ArmyManagementCalculationModel;
            _onAddToCart = onAddToCart;
            _onRemove = onRemove;
            _onFocus = onFocus;
            Party = mobileParty;
            ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(mobileParty.LeaderHero.ClanBanner), nineGrid: true);
            CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(mobileParty.LeaderHero.CharacterObject);
            LordFace = new ImageIdentifierVM(characterCode);
            Relation = armyManagementCalculationModel.GetPartyRelation(mobileParty.LeaderHero);
            Strength = Party.MemberRoster.TotalManCount;
            _distance = Campaign.Current.Models.MapDistanceModel.GetDistance(Party, newLeader.PartyBelongedTo);
            DistInTime = MathF.Ceiling(_distance / Party.Speed);
            Clan = mobileParty.LeaderHero.Clan;
            IsMainHero = mobileParty.IsMainParty;
            Cost = armyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, mobileParty);
            if (Clan.Kingdom.Leader != Hero.MainHero && mobileParty.Army.LeaderParty != MobileParty.MainParty)
            {
                Cost *= 2;
            }
            IsTransferDisabled = IsMainHero || PlayerSiege.PlayerSiegeEvent != null;
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            LeaderNameText = Party.LeaderHero.Name.ToString();
            NameText = Party.Name.ToString();
            if (!Party.IsMainParty)
            {
                DistanceText = _distance < 5 ? GameTexts.FindText("str_nearby").ToString() : CampaignUIHelper.GetPartyDistanceByTimeText((int)_distance, Party.Speed);
            }
            IsEligible = GetIsEligibleWithReason(out TextObject explanation);
            EligibleText = explanation;
        }

        public void ExecuteAction()
        {
            if (IsInCart)
            {
                OnRemove();
            }
            else
            {
                OnAddToCart();
            }
        }

        private void OnRemove()
        {
            if (!IsMainHero)
            {
                _onRemove(this);
            }
            RefreshValues();
        }

        private void OnAddToCart()
        {
            if (IsEligible)
            {
                _onAddToCart(this);
            }
            RefreshValues();
        }

        public void ExecuteSetFocused()
        {
            IsFocused = true;
            _onFocus?.Invoke(this);
        }

        public void ExecuteSetUnfocused()
        {
            IsFocused = false;
            _onFocus?.Invoke(null);
        }

        public bool GetIsEligibleWithReason(out TextObject explanation)
        {
            if (PlayerSiege.PlayerSiegeEvent != null)
            {
                explanation = GameTexts.FindText("str_action_disabled_reason_siege");
                return false;
            }
            if (Party == null)
            {
                explanation = new TextObject("{=f6vTzVar}Does not have a mobile party.");
                return false;
            }
            if (Party.Army != null && Party.Army.LeaderParty == Party.LeaderHero.PartyBelongedTo)
            {
                explanation = GameTexts.FindText("str_ineligible_army_leader_already_leading");
                explanation.SetCharacterProperties("HERO", Party.LeaderHero.CharacterObject);
                return false;
            }
            if (Party.MapEvent != null || Party.SiegeEvent != null)
            {
                explanation = new TextObject("{=pkbUiKFJ}Currently fighting an enemy.");
                return false;
            }
            if (IsInCart)
            {
                explanation = new TextObject("{=idRXFzQ6}Already added to the army.");
                return false;
            }
            explanation = TextObject.Empty;
            return true;
        }

        public void ExecuteBeginHint()
        {
            if (!IsEligible)
            {
                MBInformationManager.ShowHint(EligibleText.ToString());
                return;
            }

            InformationManager.ShowTooltip(typeof(MobileParty), Party, true, true);
        }

        public void ExecuteEndHint()
        {
            MBInformationManager.HideInformations();
        }

        public void ExecuteOpenEncyclopedia()
        {
            if (Party?.LeaderHero != null)
            {
                Campaign.Current.EncyclopediaManager.GoToLink(Party.LeaderHero.EncyclopediaLink);
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
                if (value != _removeInputKey)
                {
                    _removeInputKey = value;
                    OnPropertyChangedWithValue(value, "RemoveInputKey");
                }
            }
        }

        [DataSourceProperty]
        public bool IsEligible
        {
            get
            {
                return _isEligible;
            }
            set
            {
                if (value != _isEligible)
                {
                    _isEligible = value;
                    OnPropertyChangedWithValue(value, "IsPartyEligibleForArmy");
                }
            }
        }

        [DataSourceProperty]
        public TextObject EligibleText
        {
            get
            {
                return _eligibleText;
            }
            set
            {
                if (value != _eligibleText)
                {
                    _eligibleText = value;
                    OnPropertyChangedWithValue(value, "EligibleText");
                }
            }
        }

        [DataSourceProperty]
        public bool IsInCart
        {
            get
            {
                return _isInCart;
            }
            set
            {
                if (value != _isInCart)
                {
                    _isInCart = value;
                    OnPropertyChangedWithValue(value, "IsInCart");
                }
            }
        }

        [DataSourceProperty]
        public bool IsMainHero
        {
            get
            {
                return _isMainHero;
            }
            set
            {
                if (value != _isMainHero)
                {
                    _isMainHero = value;
                    OnPropertyChangedWithValue(value, "IsMainHero");
                }
            }
        }

        [DataSourceProperty]
        public int Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                if (value != _strength)
                {
                    _strength = value;
                    OnPropertyChangedWithValue(value, "Strength");
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
        public int Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                if (value != _cost)
                {
                    _cost = value;
                    OnPropertyChangedWithValue(value, "Cost");
                }
            }
        }

        [DataSourceProperty]
        public int Relation
        {
            get
            {
                return _relation;
            }
            set
            {
                if (value != _relation)
                {
                    _relation = value;
                    OnPropertyChangedWithValue(value, "Relation");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM ClanBanner
        {
            get
            {
                return _clanBanner;
            }
            set
            {
                if (value != _clanBanner)
                {
                    _clanBanner = value;
                    OnPropertyChangedWithValue(value, "ClanBanner");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM LordFace
        {
            get
            {
                return _lordFace;
            }
            set
            {
                if (value != _lordFace)
                {
                    _lordFace = value;
                    OnPropertyChangedWithValue(value, "LordFace");
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
        public bool IsTransferDisabled
        {
            get
            {
                return _isTransferDisabled;
            }
            set
            {
                if (value != _isTransferDisabled)
                {
                    _isTransferDisabled = value;
                    OnPropertyChangedWithValue(value, "IsTransferDisabled");
                }
            }
        }

        [DataSourceProperty]
        public string LeaderNameText
        {
            get
            {
                return _leaderNameText;
            }
            set
            {
                if (value != _leaderNameText)
                {
                    _leaderNameText = value;
                    OnPropertyChangedWithValue(value, "LeaderNameText");
                }
            }
        }

        [DataSourceProperty]
        public bool IsFocused
        {
            get
            {
                return _isFocused;
            }
            set
            {
                if (value != _isFocused)
                {
                    _isFocused = value;
                    OnPropertyChangedWithValue(value, "IsFocused");
                }
            }
        }
    }
}
