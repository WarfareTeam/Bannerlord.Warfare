using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Bannerlord.Warfare.Models
{
    public class WarfarePartySizeLimitModel : PartySizeLimitModel
    {
        private readonly PartySizeLimitModel? _model;

        public WarfarePartySizeLimitModel(PartySizeLimitModel? model)
        {
            _model = model;
        }
        public override TroopRoster FindAppropriateInitialRosterForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
        {
            if (party != null && party.IsActive && party.IsLordParty && party.ActualClan.IsMinorFaction && party != MobileParty.MainParty)
            {
                return TroopRoster.CreateDummyTroopRoster();
            }
            return _model.FindAppropriateInitialRosterForMobileParty(party, partyTemplate);
        }

        public override List<Ship> FindAppropriateInitialShipsForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
        {
            if (party != null && party.IsActive && party.IsLordParty && party.ActualClan.IsMinorFaction && party != MobileParty.MainParty)
            {
                return new List<Ship>();
            }
            return _model.FindAppropriateInitialShipsForMobileParty(party, partyTemplate);
        }


        public override int MinimumNumberOfVillagersAtVillagerParty => _model.MinimumNumberOfVillagersAtVillagerParty;

        public override ExplainedNumber CalculateGarrisonPartySizeLimit(Settlement settlement, bool includeDescriptions = false) => _model.CalculateGarrisonPartySizeLimit(settlement, includeDescriptions);

        public override int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan) => _model.GetAssumedPartySizeForLordParty(leaderHero, partyMapFaction, actualClan);

        public override int GetClanTierPartySizeEffectForHero(Hero hero) => _model.GetClanTierPartySizeEffectForHero(hero);

        public override int GetIdealVillagerPartySize(Village village) => _model.GetIdealVillagerPartySize(village);

        public override int GetNextClanTierPartySizeEffectChangeForHero(Hero hero) => _model.GetNextClanTierPartySizeEffectChangeForHero(hero);

        public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false) => _model.GetPartyMemberSizeLimit(party, includeDescriptions);

        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false) => _model.GetPartyPrisonerSizeLimit(party, includeDescriptions);
    }
}
