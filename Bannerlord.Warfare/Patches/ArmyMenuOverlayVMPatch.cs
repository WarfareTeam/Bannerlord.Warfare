using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Armies;

using Warfare.GauntletUI;
using Warfare.Helpers;
using Warfare.ViewModels.Military;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(ArmyMenuOverlayVM), "ExecuteOpenArmyManagement")]
    public static class ArmyMenuOverlayVMPatch
    {
        public static bool Prefix(ArmyMenuOverlayVM __instance)
        {
            if (MobileParty.MainParty?.Army != null)
            {
                KingdomMilitaryVM vm = VMHelper.Military;
                vm.CurrentSelectedArmy = new KingdomArmyItemVM(MobileParty.MainParty.Army, vm.OnArmySelection);
                new KingdomInterface().ShowInterface(vm.RefreshArmyList);
            }
            return false;
        }
    }
}