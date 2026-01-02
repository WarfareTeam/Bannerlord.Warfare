using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;

using HarmonyLib;

using Warfare.Helpers;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(KingdomManagementVM), MethodType.Constructor, new Type[]
    {
        typeof(Action),
        typeof(Action),
        typeof(Action<Army>)
    })]
    public class KingdomManagementVMPatch
    {
        private static void Postfix(ref int ____categoryCount)
        {
            ____categoryCount = 6;
        }
    }
    [HarmonyPatch(typeof(KingdomManagementVM), "RefreshValues")]
    public static class RefreshValuesPatch
    {
        public static void Postfix()
        {
            if (VMHelper.World != null)
            {
                VMHelper.World.RefreshValues();
            }
        }
    }
    [HarmonyPatch(typeof(KingdomManagementVM), "OnRefresh")]
    public static class OnRefreshPatch
    {
        public static void Postfix()
        {
            if (VMHelper.World != null)
            {
                VMHelper.World.RefreshWorldList();
            }
        }
    }

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
        public static void Postfix(KingdomManagementVM __instance, ref int ____currentCategory, int index)
        {
            VMHelper.Military.Show = index == 3;
            VMHelper.World.Show = index == 5;
            if (VMHelper.Military.Show || VMHelper.World.Show)
            {
                __instance.Clan.Show = false;
                __instance.Settlement.Show = false;
                __instance.Policy.Show = false;
                __instance.Army.Show = false;
                __instance.Diplomacy.Show = false;
            }
            ____currentCategory = index;
        }
    }
}