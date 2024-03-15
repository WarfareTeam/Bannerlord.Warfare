using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;

using HarmonyLib;

using Warfare.Helpers;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(KingdomManagementVM), "OnFrameTick")]
    public static class OnFrameTickPatch
    {
        public static void Postfix()
        {
            if (VMHelper.Military != null)
            {
                VMHelper.Military.OnFrameTick();
            }
        }
    }

    [HarmonyPatch(typeof(KingdomManagementVM), "SetSelectedCategory")]
    public static class SetSelectedCategoryPatch
    {
        public static void Postfix(KingdomManagementVM __instance, int index)
        {
            if (VMHelper.Military != null)
            {
                if (index == 3)
                {
                    __instance.Army.Show = false;
                    VMHelper.Military.Show = true;
                }
                else
                {
                    VMHelper.Military.Show = false;
                }
            }
        }
    }
}