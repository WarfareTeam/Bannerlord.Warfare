using SandBox.View.Map;
using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

using Warfare.Helpers;
using Warfare.ViewModels.ArmyManagement;

namespace Warfare.GauntletUI
{
    public class KingdomInterface
    {
        private GauntletLayer _layer = default!;
        private ScreenBase _screenBase = default!;
        private ViewModel? _vm;
        private SpriteCategory _category;
        private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;

        public void ShowInterface(Action onFinalize, Hero newLeader = null)
        {
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
            _category = spriteData.SpriteCategories["ui_armymanagement"];
            _layer = new GauntletLayer(300);
            if (newLeader == null)
            {
                ShowArmyManagementInterface(onFinalize);
            }
            else
            {
                ShowSplitArmyInterface(onFinalize, newLeader);
            }
            _category.Load(resourceContext, uiResourceDepot);
            _screenBase = ScreenManager.TopScreen;
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
            _layer.InputRestrictions.SetInputRestrictions();
            _layer.IsFocusLayer = true;
            _screenBase.AddLayer(_layer);
            ScreenManager.TrySetFocus(_layer);
            if (newLeader == null)
            {
                _timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
                Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                Campaign.Current.SetTimeControlModeLock(true);
                MapScreen mapScreen = ScreenManager.TopScreen as MapScreen;
                if (mapScreen != null)
                {
                    mapScreen.SetIsInArmyManagement(true);
                }
            }
        }

        public void ShowArmyManagementInterface(Action onFinalize)
        {
            KingdomArmyManagementVM vm = (KingdomArmyManagementVM)(_vm = new KingdomArmyManagementVM(OnClose, onFinalize));
            vm.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
            vm.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
            vm.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
            vm.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
            _layer.LoadMovie("ArmyManagement", vm);
        }

        public void ShowSplitArmyInterface(Action onFinalize, Hero newLeader)
        {
            SplitArmyVM vm = (SplitArmyVM)(_vm = new SplitArmyVM(VMHelper.Military.CurrentSelectedArmy.Army, newLeader, OnClose, onFinalize));
            vm.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
            vm.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
            vm.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
            vm.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
            _layer.LoadMovie("SplitArmy", vm);
        }

        public void OnClose()
        {
            _layer.InputRestrictions.ResetInputRestrictions();
            _layer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_layer);
            _screenBase.RemoveLayer(_layer);
            _layer = null!;
            _vm.OnFinalize();
            _vm = null;
            _category.Unload();
            _screenBase = null!;
            MapScreen mapScreen = ScreenManager.TopScreen as MapScreen;
            if (mapScreen != null)
            {
                mapScreen.SetIsInArmyManagement(false);
            }
            Campaign.Current.SetTimeControlModeLock(false);
            Campaign.Current.TimeControlMode = _timeControlModeBeforeArmyManagementOpened;
        }

        public GauntletLayer Layer
        {
            get
            {
                return _layer;
            }
        }

        public ViewModel ViewModel
        {
            get
            {
                return _vm;
            }
        }
    }
}
