using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace Warfare.Models
{
    public class WarfareArmyManagementCalculationModel : ArmyManagementCalculationModel
    {
        private readonly ArmyManagementCalculationModel? _model;

        public WarfareArmyManagementCalculationModel(ArmyManagementCalculationModel? model)
        {
            _model = model;
        }

        public override int CalculatePartyInfluenceCost(MobileParty armyLeaderParty, MobileParty party)
        {
            if (party.ActualClan != null && !party.ActualClan.IsEliminated && party.ActualClan.IsMinorFaction && party != MobileParty.MainParty)
            {
                if (Settings.Current.FactionLeadersCallMercenaryToArmyWithoutCost && armyLeaderParty.LeaderHero == armyLeaderParty.MapFaction.Leader)
                {
                    return 0;
                }
                return (int)(_model.CalculatePartyInfluenceCost(armyLeaderParty, party) * Settings.Current.CostMultiplerToCallMercenaryToArmy);
            }
            return _model.CalculatePartyInfluenceCost(armyLeaderParty, party);
        }

        public override int InfluenceValuePerGold => _model.InfluenceValuePerGold;

        public override int AverageCallToArmyCost => _model.AverageCallToArmyCost;

        public override int CohesionThresholdForDispersion => _model.CohesionThresholdForDispersion;

        public override ExplainedNumber CalculateDailyCohesionChange(Army army, bool includeDescriptions = false)
        {
            if (!Settings.Current.EnableCohesionChange)
            {
                return new ExplainedNumber(0f, includeDescriptions);
            }
            return _model.CalculateDailyCohesionChange(army, includeDescriptions);
        }

        public override int CalculateNewCohesion(Army army, PartyBase newParty, int calculatedCohesion, int sign) => _model.CalculateNewCohesion(army, newParty, calculatedCohesion, sign);

        public override int CalculateTotalInfluenceCost(Army army, float percentage) => _model.CalculateTotalInfluenceCost(army, percentage);

        public override bool CheckPartyEligibility(MobileParty party) => _model.CheckPartyEligibility(party);

        public override float DailyBeingAtArmyInfluenceAward(MobileParty armyMemberParty) => _model.DailyBeingAtArmyInfluenceAward(armyMemberParty);

        public override int GetCohesionBoostGoldCost(Army army, float percentageToBoost = 100) => _model.GetCohesionBoostGoldCost(army, percentageToBoost);

        public override int GetCohesionBoostInfluenceCost(Army army, int percentageToBoost = 100) => _model.GetCohesionBoostInfluenceCost(army, percentageToBoost);

        public override List<MobileParty> GetMobilePartiesToCallToArmy(MobileParty leaderParty) => _model.GetMobilePartiesToCallToArmy(leaderParty);

        public override int GetPartyRelation(Hero hero) => _model.GetPartyRelation(hero);

        public override float GetPartySizeScore(MobileParty party) => _model.GetPartySizeScore(party);

        public override int GetPartyStrength(PartyBase party) => _model.GetPartyStrength(party);
    }
}
