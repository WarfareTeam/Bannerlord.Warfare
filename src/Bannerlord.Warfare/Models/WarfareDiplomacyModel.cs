using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Bannerlord.Warfare.Models
{
    public class WarfareDiplomacyModel : DiplomacyModel
    {
        private readonly DiplomacyModel? _model;

        public WarfareDiplomacyModel(DiplomacyModel? model)
        {
            _model = model;
        }

        //Mercenaries do not go to war independently with this mod.
        public override bool IsAtConstantWar(IFaction faction1, IFaction faction2)
        {
            if ((faction1.IsOutlaw && faction1.IsMinorFaction) || (faction2.IsOutlaw && faction2.IsMinorFaction))
            {
                return false;
            }
            return _model.IsAtConstantWar(faction1, faction2);
        }

        public override int MaxRelationLimit => _model.MaxRelationLimit;

        public override int MinRelationLimit => _model.MinRelationLimit;

        public override int MaxNeutralRelationLimit => _model.MaxNeutralRelationLimit;

        public override int MinNeutralRelationLimit => _model.MinNeutralRelationLimit;

        public override int MinimumRelationWithConversationCharacterToJoinKingdom => _model.MinimumRelationWithConversationCharacterToJoinKingdom;

        public override int GiftingTownRelationshipBonus => _model.GiftingTownRelationshipBonus;

        public override int GiftingCastleRelationshipBonus => _model.GiftingCastleRelationshipBonus;

        public override float WarDeclarationScorePenaltyAgainstAllies => _model.WarDeclarationScorePenaltyAgainstAllies;

        public override float WarDeclarationScoreBonusAgainstEnemiesOfAllies => _model.WarDeclarationScoreBonusAgainstEnemiesOfAllies;

        public override bool CanSettlementBeGifted(Settlement settlement) => _model.CanSettlementBeGifted(settlement);

        public override float DenarsToInfluence() => _model.DenarsToInfluence();

        public override IEnumerable<BarterGroup> GetBarterGroups() => _model.GetBarterGroups();

        public override int GetBaseRelation(Hero hero, Hero hero1) => _model.GetBaseRelation(hero, hero1);

        public override int GetCharmExperienceFromRelationGain(Hero hero, float relationChange, ChangeRelationAction.ChangeRelationDetail detail) => _model.GetCharmExperienceFromRelationGain(hero, relationChange, detail);

        public override float GetClanStrength(Clan clan) => _model.GetClanStrength(clan);

        public override int GetDailyTributeToPay(Clan factionToPay, Clan factionToReceive, out int tributeDurationInDays) => _model.GetDailyTributeToPay(factionToPay, factionToReceive, out tributeDurationInDays);

        public override float GetDecisionMakingThreshold(IFaction consideringFaction) => _model.GetDecisionMakingThreshold(consideringFaction);

        public override DiplomacyStance GetDefaultDiplomaticStance(IFaction faction1, IFaction faction2) => _model.GetDefaultDiplomaticStance(faction1, faction2);

        public override int GetEffectiveRelation(Hero hero, Hero hero1) => _model.GetEffectiveRelation(hero, hero1);

        public override float GetHeroCommandingStrengthForClan(Hero hero) => _model.GetHeroCommandingStrengthForClan(hero);

        public override void GetHeroesForEffectiveRelation(Hero hero1, Hero hero2, out Hero effectiveHero1, out Hero effectiveHero2) => _model.GetHeroesForEffectiveRelation(hero1, hero2, out effectiveHero1, out effectiveHero2);

        public override float GetHeroGoverningStrengthForClan(Hero hero) => _model.GetHeroGoverningStrengthForClan(hero);

        public override float GetHourlyInfluenceAwardForBeingArmyMember(MobileParty mobileParty) => _model.GetHourlyInfluenceAwardForBeingArmyMember(mobileParty);

        public override float GetHourlyInfluenceAwardForBesiegingEnemyFortification(MobileParty mobileParty) => _model.GetHourlyInfluenceAwardForBesiegingEnemyFortification(mobileParty);

        public override float GetHourlyInfluenceAwardForRaidingEnemyVillage(MobileParty mobileParty) => _model.GetHourlyInfluenceAwardForRaidingEnemyVillage(mobileParty);

        public override int GetInfluenceAwardForSettlementCapturer(Settlement settlement) => _model.GetInfluenceAwardForSettlementCapturer(settlement);

        public override int GetInfluenceCostOfAbandoningArmy() => _model.GetInfluenceCostOfAbandoningArmy();

        public override int GetInfluenceCostOfAnnexation(Clan proposingClan) => _model.GetInfluenceCostOfAnnexation(proposingClan);

        public override int GetInfluenceCostOfChangingLeaderOfArmy() => _model.GetInfluenceCostOfChangingLeaderOfArmy();

        public override int GetInfluenceCostOfDisbandingArmy() => _model.GetInfluenceCostOfDisbandingArmy();

        public override int GetInfluenceCostOfExpellingClan(Clan proposingClan) => _model.GetInfluenceCostOfExpellingClan(proposingClan);

        public override int GetInfluenceCostOfPolicyProposalAndDisavowal(Clan proposingClan) => _model.GetInfluenceCostOfPolicyProposalAndDisavowal(proposingClan);

        public override int GetInfluenceCostOfProposingPeace(Clan proposingClan) => _model.GetInfluenceCostOfProposingPeace(proposingClan);

        public override int GetInfluenceCostOfProposingWar(Clan proposingClan) => _model.GetInfluenceCostOfProposingWar(proposingClan);

        public override int GetInfluenceCostOfSupportingClan() => _model.GetInfluenceCostOfSupportingClan();

        public override int GetInfluenceValueOfSupportingClan() => _model.GetInfluenceValueOfSupportingClan();

        public override uint GetNotificationColor(ChatNotificationType notificationType) => _model.GetNotificationColor(notificationType);

        public override int GetRelationChangeAfterClanLeaderIsDead(Hero deadLeader, Hero relationHero) => _model.GetRelationChangeAfterClanLeaderIsDead(deadLeader, relationHero);

        public override int GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(Hero supporter, bool hasHeroVotedAgainstOwner) => _model.GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(supporter, hasHeroVotedAgainstOwner);

        public override int GetRelationCostOfDisbandingArmy(bool isLeaderParty) => _model.GetRelationCostOfDisbandingArmy(isLeaderParty);

        public override int GetRelationCostOfExpellingClanFromKingdom() => _model.GetRelationCostOfExpellingClanFromKingdom();

        public override float GetRelationIncreaseFactor(Hero hero1, Hero hero2, float relationValue) => _model.GetRelationIncreaseFactor(hero1, hero2, relationValue);

        public override int GetRelationValueOfSupportingClan() => _model.GetRelationValueOfSupportingClan();

        public override float GetScoreOfClanToJoinKingdom(Clan clan, Kingdom kingdom) => _model.GetScoreOfClanToJoinKingdom(clan, kingdom);

        public override float GetScoreOfClanToLeaveKingdom(Clan clan, Kingdom kingdom) => _model.GetScoreOfClanToLeaveKingdom(clan, kingdom);

        public override float GetScoreOfDeclaringPeace(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace) => _model.GetScoreOfDeclaringPeace(factionDeclaresPeace, factionDeclaredPeace);

        public override float GetScoreOfDeclaringPeaceForClan(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace, Clan evaluatingClan, out TextObject reason, bool includeReason = false) => _model.GetScoreOfDeclaringPeaceForClan(factionDeclaresPeace, factionDeclaredPeace, evaluatingClan, out reason, includeReason);

        public override float GetScoreOfDeclaringWar(IFaction factionDeclaresWar, IFaction factionDeclaredWar, Clan evaluatingClan, out TextObject reason, bool includeReason = false) => _model.GetScoreOfDeclaringWar(factionDeclaresWar, factionDeclaredWar, evaluatingClan, out reason, includeReason);

        public override float GetScoreOfKingdomToGetClan(Kingdom kingdom, Clan clan) => _model.GetScoreOfKingdomToGetClan(kingdom, clan);

        public override float GetScoreOfKingdomToHireMercenary(Kingdom kingdom, Clan mercenaryClan) => _model.GetScoreOfKingdomToHireMercenary(kingdom, mercenaryClan);

        public override float GetScoreOfKingdomToSackClan(Kingdom kingdom, Clan clan) => _model.GetScoreOfKingdomToSackClan(kingdom, clan);

        public override float GetScoreOfKingdomToSackMercenary(Kingdom kingdom, Clan mercenaryClan) => _model.GetScoreOfKingdomToSackMercenary(kingdom, mercenaryClan);

        public override float GetScoreOfLettingPartyGo(MobileParty party, MobileParty partyToLetGo) => _model.GetScoreOfLettingPartyGo(party, partyToLetGo);

        public override float GetScoreOfMercenaryToJoinKingdom(Clan clan, Kingdom kingdom) => _model.GetScoreOfMercenaryToJoinKingdom(clan, kingdom);

        public override float GetScoreOfMercenaryToLeaveKingdom(Clan clan, Kingdom kingdom) => _model.GetScoreOfMercenaryToLeaveKingdom(clan, kingdom);

        public override DiplomacyStance? GetShallowDiplomaticStance(IFaction faction1, IFaction faction2) => _model.GetShallowDiplomaticStance(faction1, faction2);

        public override float GetStrengthThresholdForNonMutualWarsToBeIgnoredToJoinKingdom(Kingdom kingdomToJoin) => _model.GetStrengthThresholdForNonMutualWarsToBeIgnoredToJoinKingdom(kingdomToJoin);

        public override float GetValueOfHeroForFaction(Hero examinedHero, IFaction targetFaction, bool forMarriage = false) => _model.GetValueOfHeroForFaction(examinedHero, targetFaction, forMarriage);

        public override float GetValueOfSettlementsForFaction(IFaction faction) => _model.GetValueOfSettlementsForFaction(faction);

        public override ExplainedNumber GetWarProgressScore(IFaction factionDeclaresWar, IFaction factionDeclaredWar, bool includeDescriptions = false) => _model.GetWarProgressScore(factionDeclaresWar, factionDeclaredWar, includeDescriptions);

        public override bool IsClanEligibleToBecomeRuler(Clan clan) => _model.IsClanEligibleToBecomeRuler(clan);

        public override bool IsPeaceSuitable(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace) => _model.IsPeaceSuitable(factionDeclaresPeace, factionDeclaredPeace);
    }
}
