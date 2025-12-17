using System;
using System.Collections.Generic;

using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Warfare.ViewModels.World
{
    public class WorldWarItemVM : WorldDiplomacyItemVM
    {
        private readonly Action<WorldWarItemVM> _onSelect;
        private readonly StanceLink _war;
        private readonly Action<WorldWarItemVM> _onAction;
        private ExplainedNumber _warProgressOfFaction1;
        private ExplainedNumber _warProgressOfFaction2;
        private int _numberOfTownsCapturedByFaction1;
        private int _numberOfTownsCapturedByFaction2;
        private int _numberOfCastlesCapturedByFaction1;
        private int _numberOfCastlesCapturedByFaction2;
        private int _numberOfRaidsMadeByFaction1;
        private int _numberOfRaidsMadeByFaction2;
        private string _warName;
        private string _numberOfDaysSinceWarBegan;
        private int _score;
        private bool _isBehaviorSelectionEnabled;
        private int _casualtiesOfFaction1;
        private int _casualtiesOfFaction2;
        private MBBindingList<KingdomWarLogItemVM> _warLog;

        public WorldWarItemVM(StanceLink war, Action<WorldWarItemVM> onSelect)
            : base(war.Faction1, war.Faction2)
        {
            _war = war;
            _onSelect = onSelect;
            _onSelect = onSelect;
            IsBehaviorSelectionEnabled = Faction1.IsKingdomFaction && Faction1.Leader == Hero.MainHero;
            StanceLink stanceWith = Faction1.GetStanceWith(Faction2);
            _warProgressOfFaction1 = Campaign.Current.Models.DiplomacyModel.GetWarProgressScore(Faction1, Faction2, true);
            _warProgressOfFaction2 = Campaign.Current.Models.DiplomacyModel.GetWarProgressScore(Faction2, Faction1, true);
            _numberOfTownsCapturedByFaction1 = stanceWith.GetSuccessfulTownSieges(Faction1);
            _numberOfTownsCapturedByFaction2 = stanceWith.GetSuccessfulTownSieges(Faction2);
            _numberOfCastlesCapturedByFaction1 = stanceWith.GetSuccessfulSieges(Faction1) - _numberOfTownsCapturedByFaction1;
            _numberOfCastlesCapturedByFaction2 = stanceWith.GetSuccessfulSieges(Faction2) - _numberOfTownsCapturedByFaction2;
            _numberOfRaidsMadeByFaction1 = stanceWith.GetSuccessfulRaids(Faction1);
            _numberOfRaidsMadeByFaction2 = stanceWith.GetSuccessfulRaids(Faction2);
            RefreshValues();
            WarLog = new MBBindingList<KingdomWarLogItemVM>();
            foreach (var (logEntry, effectorFaction, _) in DiplomacyHelper.GetLogsForWar(war))
            {
                if (logEntry is IEncyclopediaLog log)
                {
                    WarLog.Add(new KingdomWarLogItemVM(log, effectorFaction));
                }
            }
        }
        public override void RefreshValues()
        {
            base.RefreshValues();
            UpdateDiplomacyProperties();
        }
        protected override void OnSelect()
        {
            UpdateDiplomacyProperties();
            _onSelect(this);
            IsSelected = true;
        }
        protected override void UpdateDiplomacyProperties()
        {
            base.UpdateDiplomacyProperties();
            GameTexts.SetVariable("FACTION_1_NAME", Faction1.Name.ToString());
            GameTexts.SetVariable("FACTION_2_NAME", Faction2.Name.ToString());
            WarName = GameTexts.FindText("str_war_faction_versus_faction").ToString();
            StanceLink stanceWith = Faction1.GetStanceWith(Faction2);
            Score = stanceWith.GetSuccessfulSieges(Faction1) + stanceWith.GetSuccessfulRaids(Faction1);
            CasualtiesOfFaction1 = stanceWith.GetCasualties(Faction1);
            CasualtiesOfFaction2 = stanceWith.GetCasualties(Faction2);
            int num = MathF.Ceiling(_war.WarStartDate.ElapsedDaysUntilNow + 0.01f);
            TextObject textObject = GameTexts.FindText("str_for_DAY_days");
            textObject.SetTextVariable("DAY", num.ToString());
            textObject.SetTextVariable("DAY_IS_PLURAL", (num > 1) ? 1 : 0);
            NumberOfDaysSinceWarBegan = textObject.ToString();

            Stats.Add(new KingdomWarComparableStatVM((int)Faction1.CurrentTotalStrength, (int)Faction2.CurrentTotalStrength, GameTexts.FindText("str_total_strength"), _faction1Color, _faction2Color, 10000));
            Stats.Add(new KingdomWarComparableStatVM(stanceWith.GetCasualties(Faction2), stanceWith.GetCasualties(Faction1), GameTexts.FindText("str_war_casualties_inflicted"), _faction1Color, _faction2Color, 10000));
            Stats.Add(new KingdomWarComparableStatVM(_numberOfTownsCapturedByFaction1, _numberOfTownsCapturedByFaction2, GameTexts.FindText("str_war_captured_towns", null), _faction1Color, _faction2Color, 25));
            Stats.Add(new KingdomWarComparableStatVM(_numberOfCastlesCapturedByFaction1, _numberOfCastlesCapturedByFaction2, GameTexts.FindText("str_war_captured_castles", null), _faction1Color, _faction2Color, 25));
            Stats.Add(new KingdomWarComparableStatVM(_numberOfRaidsMadeByFaction1, _numberOfRaidsMadeByFaction2, GameTexts.FindText("str_war_successful_raids", null), _faction1Color, _faction2Color, 10));
        }
        protected override void ExecuteAction()
        {
            _onAction(this);
        }

        [DataSourceProperty]
        public string WarName
        {
            get
            {
                return _warName;
            }
            set
            {
                if (value != _warName)
                {
                    _warName = value;
                    OnPropertyChangedWithValue(value, "WarName");
                }
            }
        }
        [DataSourceProperty]
        public string NumberOfDaysSinceWarBegan
        {
            get
            {
                return _numberOfDaysSinceWarBegan;
            }
            set
            {
                if (value != _numberOfDaysSinceWarBegan)
                {
                    _numberOfDaysSinceWarBegan = value;
                    OnPropertyChangedWithValue(value, "NumberOfDaysSinceWarBegan");
                }
            }
        }
        [DataSourceProperty]
        public bool IsBehaviorSelectionEnabled
        {
            get
            {
                return _isBehaviorSelectionEnabled;
            }
            set
            {
                if (value != _isBehaviorSelectionEnabled)
                {
                    _isBehaviorSelectionEnabled = value;
                    OnPropertyChangedWithValue(value, "IsBehaviorSelectionEnabled");
                }
            }
        }
        [DataSourceProperty]
        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (value != _score)
                {
                    _score = value;
                    OnPropertyChangedWithValue(value, "Score");
                }
            }
        }
        [DataSourceProperty]
        public int CasualtiesOfFaction1
        {
            get
            {
                return _casualtiesOfFaction1;
            }
            set
            {
                if (value != _casualtiesOfFaction1)
                {
                    _casualtiesOfFaction1 = value;
                    OnPropertyChangedWithValue(value, "CasualtiesOfFaction1");
                }
            }
        }
        [DataSourceProperty]
        public int CasualtiesOfFaction2
        {
            get
            {
                return _casualtiesOfFaction2;
            }
            set
            {
                if (value != _casualtiesOfFaction2)
                {
                    _casualtiesOfFaction2 = value;
                    OnPropertyChangedWithValue(value, "CasualtiesOfFaction2");
                }
            }
        }
        [DataSourceProperty]
        public MBBindingList<KingdomWarLogItemVM> WarLog
        {
            get
            {
                return _warLog;
            }
            set
            {
                if (value != _warLog)
                {
                    _warLog = value;
                    OnPropertyChangedWithValue(value, "WarLog");
                }
            }
        }
    }
}