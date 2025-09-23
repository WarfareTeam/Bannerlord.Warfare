using SandBox.GauntletUI.Map;

using HarmonyLib;

using Warfare.GauntletUI;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(GauntletMapBarGlobalLayer), "OpenArmyManagement")]
    public static class GauntletMapBarGlobalLayerPatch
    {
        public static bool Prefix()
        {
            new KingdomInterface().ShowInterface();
            return false;
        }
    }
}
