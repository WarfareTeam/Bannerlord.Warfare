using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.MountAndBlade;
using Warfare;

namespace Bannerlord.Warfare.Patches
{
    [HarmonyPatch(typeof(BannerlordConfig), "GetRealBattleSize")]
    public class GetRealBattleSizePatch
    {
        private static void Postfix(ref int __result)
        {
            if (Settings.Current.EnableModifyMaximumBattlefieldAgents)
            {
                __result = PartyGroupTroopSupplierPatch.GetMaximumTroops();
            }
        }
    }
    [HarmonyPatch(typeof(BannerlordConfig), "GetRealBattleSizeForSallyOut")]
    public class GetRealBattleSizeForSallyOutPatch
    {
        private static void Postfix(ref int __result)
        {
            if (Settings.Current.EnableModifyMaximumBattlefieldAgents)
            {
                __result = PartyGroupTroopSupplierPatch.GetMaximumTroops();
            }
        }
    }
    [HarmonyPatch(typeof(BannerlordConfig), "GetRealBattleSizeForSiege")]
    public class GetRealBattleSizeForSiegePatch
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
