using System;

using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

using Warfare.ViewModels.ArmyManagement;

namespace Warfare.GauntletUI
{
    public class KingdomInterface
    {
        private GauntletLayer _layer = default!;
        private ScreenBase _screenBase = default!;
        private ViewModel? _vm;
        private SpriteCategory? _category;
        private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;

        public void ShowInterface(Action onFinalize = null!, Hero newLeader = null!)
        {
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.ResourceDepot;
            _category = spriteData.SpriteCategories["ui_armymanagement"];
            _layer = new GauntletLayer("ArmyManagement", 300);
            WarfareArmyManagementVM vm = (WarfareArmyManagementVM)(_vm = new WarfareArmyManagementVM(OnClose, onFinalize, newLeader));
            _layer.LoadMovie("ArmyManagement", vm);
            _category.Load(resourceContext, uiResourceDepot);
            _screenBase = ScreenManager.TopScreen;
            _layer.InputRestrictions.SetInputRestrictions();
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
            _layer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(_layer);
            _screenBase.AddLayer(_layer);
            vm.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
            vm.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
            vm.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
            vm.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
            _timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
            Campaign.Current.SetTimeControlModeLock(true);
            if (ScreenManager.TopScreen is MapScreen mapScreen)
            {
                mapScreen.SetIsInArmyManagement(true);
            }
        }

        public void OnClose()
        {
            _layer.InputRestrictions.ResetInputRestrictions();
            _layer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_layer);
            _screenBase.RemoveLayer(_layer);
            _layer = null!;
            _vm!.OnFinalize();
            _vm = null;
            _category!.Unload();
            _screenBase = null!;
            MapScreen mapScreen = (ScreenManager.TopScreen as MapScreen)!;
            if (mapScreen != null)
            {
                mapScreen.SetIsInArmyManagement(false);
            }
            Campaign.Current.SetTimeControlModeLock(false);
            Campaign.Current.TimeControlMode = _timeControlModeBeforeArmyManagementOpened;
        }

        public GauntletLayer Layer => _layer;

        public ViewModel ViewModel => _vm!;
    }
}
