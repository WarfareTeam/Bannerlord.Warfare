using TaleWorlds.MountAndBlade;

using HarmonyLib;

using Warfare;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(MissionAgentSpawnLogic), "MaxNumberOfAgentsForMission", MethodType.Getter)]
    public class MaxNumberOfAgentsForMissionPatch
    {
        private static void Postfix(ref int __result)
        {
            if (Settings.Current.EnableModifyMaximumBattlefieldAgents)
            {
                __result = PartyGroupTroopSupplierPatch.GetMaximumAgents();
            }
        }
    }

    [HarmonyPatch(typeof(MissionAgentSpawnLogic), "MaxNumberOfTroopsForMission", MethodType.Getter)]
    public class MaxNumberOfTroopsForMissionPatch
    {
        private static void Postfix(ref int __result)
        {
            if (Settings.Current.EnableModifyMaximumBattlefieldAgents)
            {
                __result = PartyGroupTroopSupplierPatch.GetMaximumTroops();
            }
        }
    }
}
