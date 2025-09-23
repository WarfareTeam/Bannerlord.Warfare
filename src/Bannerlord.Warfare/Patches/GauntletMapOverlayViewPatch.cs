using SandBox.GauntletUI.Map;

using HarmonyLib;

using Warfare.GauntletUI;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(GauntletMapOverlayView), "OpenArmyManagement")]
    public static class GauntletMapOverlayViewPatch
    {
        public static bool Prefix()
        {
            new KingdomInterface().ShowInterface();
            return false;
        }
    }
}
