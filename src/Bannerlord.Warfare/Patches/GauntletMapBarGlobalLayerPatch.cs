using SandBox.GauntletUI.Map;

using HarmonyLib;

using Warfare.GauntletUI;

namespace Bannerlord.Warfare.Patches
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
