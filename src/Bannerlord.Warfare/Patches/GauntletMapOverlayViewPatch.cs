using SandBox.GauntletUI.Map;

using HarmonyLib;

using Warfare.GauntletUI;

namespace Bannerlord.Warfare.Patches
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
