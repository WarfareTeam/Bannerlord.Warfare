using System.Collections.Generic;
using System.Linq;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

using HarmonyLib;

namespace Warfare.Patches
{
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

    [HarmonyPatch(typeof(SetPartyAiAction), "GetActionForBesiegingSettlement")]
    public static class SetPartyAiActionBesiegePatch
    {
        static List<PartyAiAction> partyAiActionQueue = new List<PartyAiAction>();

        public static bool Prefix(MobileParty owner, Settlement settlement)
        {
            Army army;
            if (!Settings.Current.ModifyArmyBesiegeAI || (army = owner.Army) == null || settlement.MapFaction.Fiefs.CountQ() < Settings.Current.ModifyArmyBesiegeAIMinimumFiefs || (owner.Ai.DefaultBehavior == AiBehavior.BesiegeSettlement && owner.TargetSettlement == settlement) || !settlement.IsFortification)
            {
                /*Use vanilla logic if:
                1. modify army ai setting is disabled
                2. attacker has no army
                3. target settlements faction has less than 3 fiefs
                4. attacker is already traveling to besiege this target
                5. if the settlement is neither a town or castle*/
                return true;
            }
            foreach (PartyAiAction action in partyAiActionQueue.ToList())
            {
                if (action.Expiration.IsFuture)
                {
                    if (army == action.Army && settlement == action.Settlement)
                    {
                        return false;
                    }
                    continue;
                }
                partyAiActionQueue.Remove(action);
            }
            float power = settlement.Parties.WhereQ(x => x.IsActive && !x.IsVillager && !x.IsCaravan).SumQ(x => x.Party.TotalStrength);
            IEnumerable<float> totalStrength = owner.ActualClan.Kingdom.Armies.WhereQ(x => x.ArmyOwner != owner.LeaderHero && (x.LeaderParty.SiegeEvent != null || (x.AIBehavior == Army.AIBehaviorFlags.TravellingToAssignment && x.ArmyType == Army.ArmyTypes.Besieger)) && (Settlement)x.AiBehaviorObject != null && (Settlement)x.AiBehaviorObject == settlement).SelectQ(x => x.TotalStrength);
            if (power >= totalStrength.SumQ(x => x))
            {
                //Force party attempting to siege to instead defend if one of their kingdoms fiefs are under attack
                //This needs heavy expansion in the future...
                IEnumerable<Settlement> settlementsUnderSiege = from x in owner.ActualClan.Kingdom.Settlements where x.SiegeEvent != null orderby x.OwnerClan == owner.ActualClan select x;
                Settlement settlementToDefend;
                if (!settlementsUnderSiege.IsEmpty() && (settlementToDefend = settlementsUnderSiege.First()) != null)
                {
                    SetPartyAiAction.GetActionForDefendingSettlement(owner, settlementToDefend);
                    AddToQueue(army, settlement);
                    return false;
                }
            }
            if (MBRandom.RandomFloat < totalStrength.SumQ(x => (x - power) / 1000f))
            {
                AddToQueue(army, settlement);
                return false;
            }
            return true;
        }

        public static void AddToQueue(Army army, Settlement settlement)
        {
            partyAiActionQueue.Add(new PartyAiAction(army, settlement, CampaignTime.HoursFromNow(Settings.Current.TimeToPreventArmyBesiegeAIHours)));
        }
    }
}
