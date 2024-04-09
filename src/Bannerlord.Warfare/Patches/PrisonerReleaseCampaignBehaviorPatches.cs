using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Actions;

using HarmonyLib;

using Warfare;
using Warfare.Behaviors;
using Warfare.Prisoners;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;

namespace Bannerlord.Warfare.Patches
{
    /*
    [HarmonyPatch(typeof(PrisonerReleaseCampaignBehavior), "ApplyEscapeChanceToExceededPrisoners")]
    public static class ApplyEscapeChanceToExceededPrisonersPatch
    {
        public static bool Prefix(CharacterObject character, MobileParty capturerParty)
        {
            ExplainedNumber explainedNumber = new ExplainedNumber(0.1f, false, null);
            if (capturerParty.HasPerk(DefaultPerks.Athletics.Stamina, true))
            {
                explainedNumber.AddFactor(-0.1f, DefaultPerks.Athletics.Stamina.Name);
            }
            if (capturerParty.IsGarrison || capturerParty.IsMilitia || character.IsPlayerCharacter)
            {
                return false;
            }
            if (MBRandom.RandomFloat < explainedNumber.ResultNumber)
            {
                if (character.IsHero)
                {
                    EndCaptivityAction.ApplyByEscape(character.HeroObject, null);
                    return false;
                }
                MobileParty ownerParty = null!;
                PrisonerBehavior behavior = Campaign.Current.GetCampaignBehavior<PrisonerBehavior>();
                IEnumerable<Prisoner> prisoners = behavior.FindPrisonersForCaptor(capturerParty.LeaderHero);
                if (prisoners != null)
                {
                    foreach (Prisoner prisoner in prisoners)
                    {
                        bool found = false;
                        foreach (FlattenedTroopRosterElement prisonerElement in prisoner.Roster.ToFlattenedRoster())
                        {
                            if (prisoner.Owner != null && prisoner.Owner.PartyBelongedTo != null && prisoner.Owner.PartyBelongedTo.LimitedPartySize > prisoner.Owner.PartyBelongedTo.MemberRoster.TotalManCount && character.StringId == prisonerElement.Troop.StringId)
                            {
                                behavior.RemovePrisoner(prisoner, prisonerElement.Troop);
                                prisoner.Owner.PartyBelongedTo.MemberRoster.AddToCounts(prisonerElement.Troop, 1);
                                ownerParty = prisoner.Owner.PartyBelongedTo;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                }
                capturerParty.PrisonRoster.AddToCounts(character, -1, false, 0, 0, true, -1);
                if (Settings.Current.Logging && ownerParty != null)
                {
                    SubModule.Log("Returned " + character.StringId + " to " + ownerParty.Name + " from captor " + capturerParty.Name);
                }
            }
            return false;
        }
    }
    */
}
