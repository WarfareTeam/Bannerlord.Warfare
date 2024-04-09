using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

using HarmonyLib;

using Warfare;
using Warfare.Behaviors;
using Warfare.Prisoners;

namespace Bannerlord.Warfare.Patches
{
    /*
    [HarmonyPatch(typeof(MapEvent), "FinishBattle")]
    public static class FinishBattlePatch
    {
        public static bool Prefix(MapEvent __instance)
        {
            PrisonerBehavior behavior = Campaign.Current.GetCampaignBehavior<PrisonerBehavior>();
            if (__instance.BattleState == BattleState.AttackerVictory)
            {
                PartyBase attackerParty = __instance.GetLeaderParty(BattleSideEnum.Defender);
                if (attackerParty != null && attackerParty.LeaderHero != null)
                {
                    Prisoner potentialPrisoners = behavior.FindPotentialPrisoners(attackerParty.Owner);
                    if (potentialPrisoners != null)
                    {
                        behavior.RemovePotentialPrisoners(potentialPrisoners);
                    }
                    if (attackerParty.MobileParty != null && attackerParty.MobileParty.AttachedParties != null)
                    {
                        foreach (MobileParty party in attackerParty.MobileParty.AttachedParties)
                        {
                            potentialPrisoners = behavior.FindPotentialPrisoners(party.Owner);
                            if (potentialPrisoners != null)
                            {
                                behavior.RemovePotentialPrisoners(potentialPrisoners);
                            }
                        }
                    }
                }
            }
            if (__instance.BattleState == BattleState.DefenderVictory)
            {
                PartyBase defenderParty = __instance.GetLeaderParty(BattleSideEnum.Defender);
                if (defenderParty != null && defenderParty.LeaderHero != null)
                {
                    Prisoner potentialPrisoners = behavior.FindPotentialPrisoners(defenderParty.Owner);
                    if (potentialPrisoners != null)
                    {
                        behavior.RemovePotentialPrisoners(potentialPrisoners);
                    }
                    if (defenderParty.MobileParty != null && defenderParty.MobileParty.AttachedParties != null)
                    {
                        foreach (MobileParty party in defenderParty.MobileParty.AttachedParties)
                        {
                            potentialPrisoners = behavior.FindPotentialPrisoners(party.Owner);
                            if (potentialPrisoners != null)
                            {
                                behavior.RemovePotentialPrisoners(potentialPrisoners);
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
    */
}
