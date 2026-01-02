using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Warfare.Helpers;
using Warfare.ViewModels.Military;
using Warfare.ViewModels.World;

namespace Warfare.ViewModelMixins
{

    [ViewModelMixin("RefreshValues", true)]
    public class KingdomManagementVMMixin : BaseViewModelMixin<KingdomManagementVM>
    {
        private string _militaryText;

        private string _worldText;

        public KingdomManagementVMMixin(KingdomManagementVM vm) : base(vm)
        {
            Military = new KingdomMilitaryVM();
            World = new KingdomWorldVM();
            MilitaryText = new TextObject("{=4T0zfjz0}Military").ToString();
            WorldText = new TextObject("{=B9Be9n7r}World").ToString();
        }

        [DataSourceMethod]
        public void ExecuteShowMilitary()
        {
            AccessTools.Method(typeof(KingdomManagementVM), "SetSelectedCategory").Invoke(ViewModel, new object[] { 3 });
        }

        [DataSourceMethod]
        public void ExecuteShowWorld()
        {
            AccessTools.Method(typeof(KingdomManagementVM), "SetSelectedCategory").Invoke(ViewModel, new object[] { 5 });
        }

        [DataSourceProperty]
        public KingdomMilitaryVM Military
        {
            get => VMHelper.Military!;
            set
            {
                if (value != VMHelper.Military)
                {
                    VMHelper.Military = value;
                    OnPropertyChangedWithValue(value, "Military");
                }
            }
        }

        [DataSourceProperty]
        public KingdomWorldVM World
        {
            get => VMHelper.World!;
            set
            {
                if (value != VMHelper.World)
                {
                    VMHelper.World = value;
                    OnPropertyChangedWithValue(value, "World");
                }
            }
        }

        [DataSourceProperty]
        public string MilitaryText
        {
            get => _militaryText;
            set
            {
                if (value != _militaryText)
                {
                    _militaryText = value;
                    OnPropertyChangedWithValue(value, "MilitaryText");
                }
            }
        }

        [DataSourceProperty]
        public string WorldText
        {
            get => _worldText;
            set
            {
                if (value != _worldText)
                {
                    _worldText = value;
                    OnPropertyChangedWithValue(value, "WorldText");
                }
            }
        }
    }
}
