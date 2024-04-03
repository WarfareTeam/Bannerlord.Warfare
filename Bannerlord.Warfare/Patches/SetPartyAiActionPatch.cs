using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

using HarmonyLib;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(SetPartyAiAction), "GetActionForBesiegingSettlement")]
    public static class SetPartyAiActionPatch
    {
        static List<PartyAiAction> aiActionQueue = new List<PartyAiAction>();

        public static bool Prefix(MobileParty owner, Settlement settlement)
        {
            if (owner.Army == null || settlement.MapFaction.Fiefs.Count() < 3 || (owner.Ai.DefaultBehavior == AiBehavior.BesiegeSettlement && owner.TargetSettlement == settlement) || !settlement.IsFortification)
            {
                //Use vanilla logic
                //if attacker has no army,
                //if the target settlements faction has less than 3 fiefs,
                //if attacker is already traveling to besiege this target,
                //if the settlement is neither a town or castle
                return true;
            }
            foreach (PartyAiAction action in aiActionQueue.ToList())
            {
                if (action.Expiration.IsFuture)
                {
                    if (action.Army == owner.Army && action.Settlement == settlement)
                    {
                        return false;
                    }
                    continue;
                }
                aiActionQueue.Remove(action);
            }
            float power = settlement.Parties.Where(x => x.IsActive && !x.IsVillager && !x.IsCaravan).Select(x => x.Party.TotalStrength).Sum();
            float score = owner.ActualClan.Kingdom.Armies.Where(x => x.ArmyOwner != owner.LeaderHero && (x.LeaderParty.SiegeEvent != null || (x.AIBehavior == Army.AIBehaviorFlags.TravellingToAssignment && x.ArmyType == Army.ArmyTypes.Besieger)) && (Settlement)x.AiBehaviorObject != null && (Settlement)x.AiBehaviorObject == settlement).Select(x => (x.TotalStrength - power) / 1000f).Sum();
            if (MBRandom.RandomFloat < score)
            {
                aiActionQueue.Add(new PartyAiAction(owner.Army, settlement, CampaignTime.HoursFromNow(CampaignTime.HoursInDay)));
                return false;
            }
            return true;
        }
        internal class PartyAiAction
        {
            public Army Army { get; private set; }

            public Settlement Settlement { get; private set; }

            public CampaignTime Expiration { get; set; }

            public PartyAiAction(Army army, Settlement settlement, CampaignTime expiration)
            {
                Army = army;
                Settlement = settlement;
                Expiration = expiration;
            }
        }
    }
}
