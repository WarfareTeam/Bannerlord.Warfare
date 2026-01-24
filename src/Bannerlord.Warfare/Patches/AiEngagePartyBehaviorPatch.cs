using HarmonyLib;
using Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using Warfare;
using Warfare.Behaviors;
using Warfare.Content.Strategies;
using Warfare.Helpers;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(AiEngagePartyBehavior), "AiHourlyTick")]
    public static class AiEngagePartyBehaviorPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codes = instructions.ToList();
            LocalBuilder tempFloat = il.DeclareLocal(typeof(float));
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];

                if (i + 2 < codes.Count && codes[i].opcode == OpCodes.Ldloc_S && codes[i + 1].opcode == OpCodes.Mul && codes[i + 2].opcode == OpCodes.Ldc_R4 && (float)codes[i + 2].operand == 2f)
                {
                    yield return new CodeInstruction(OpCodes.Stloc, tempFloat);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Ldloc, tempFloat);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AiEngagePartyBehaviorPatch), nameof(AddStrategyBehaviorScore)));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AddStrategyBehaviorScore(MobileParty mobileParty, PartyThinkParams p, float value)
        {
            if (Settings.Current.EnableStrategies && mobileParty.MapFaction != null && mobileParty.MapFaction.IsKingdomFaction && mobileParty.MapFaction.Leader == Hero.MainHero)
            {
                Strategy strategy = SubModule.StrategyBehavior.FindStrategy(mobileParty.Owner);
                if (strategy != null)
                {
                    if (strategy.Priority == 1)
                    {
                        value *= Settings.Current.ChaseTendencyDefensiveStrategy;
                    }
                    else if (strategy.Priority == 2)
                    {
                        value *= Settings.Current.ChaseTendencyOffensiveStrategy;
                    }
                }
            }
            return value;
        }
    }
}