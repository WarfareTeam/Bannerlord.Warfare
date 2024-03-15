using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace Warfare.Extensions
{
    internal static class MobilePartyExtensions
    {
        internal static IEnumerable<CharacterObject> mercenaryObjects = CharacterObject.FindAll(c => c.Occupation == Occupation.Mercenary);

        internal static void AddElementToMercenaryRoster(this MobileParty party)
        {
            CharacterObject mercenary = mercenaryObjects.GetRandomElementInefficiently();
            try
            {
                party.AddElementToMemberRoster(mercenary, 1);

            }
            catch
            {
                SubModule.Log("Could not add element to " + party.Name + " roster.");
            }
        }
    }
}
