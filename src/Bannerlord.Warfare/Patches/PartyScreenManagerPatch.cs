using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

using HarmonyLib;

using Warfare.Behaviors;
using Warfare.Prisoners;

namespace Bannerlord.Warfare.Patches
{
    /*
    [HarmonyPatch(typeof(PartyScreenManager), "DefaultDoneHandler")]
    public static class PartyScreenManagerPatch
    {
        public static void Postfix(FlattenedTroopRoster releasedPrisonerRoster, PartyBase rightParty)
        {
            PrisonerBehavior behavior = Campaign.Current.GetCampaignBehavior<PrisonerBehavior>();
            Prisoner prisoner = behavior.FindPrisonerForCaptor(rightParty.LeaderHero);
            if (prisoner != null)
            {
                behavior.ReleasePrisoners(prisoner.Owner, prisoner.Captor, releasedPrisonerRoster);
            }
        }
    }
    */
}
