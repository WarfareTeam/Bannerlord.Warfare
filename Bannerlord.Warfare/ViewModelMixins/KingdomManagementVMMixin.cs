using System.Reflection;

using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Library;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using Warfare.Helpers;
using Warfare.ViewModels.Military;

namespace Warfare.ViewModelMixins
{
    [ViewModelMixin(nameof(KingdomManagementVM))]
    public class KingdomManagementVMMixin : BaseViewModelMixin<KingdomManagementVM>
    {
        private string _militaryText;

        public KingdomManagementVMMixin(KingdomManagementVM vm) : base(vm)
        {
            Military = new KingdomMilitaryVM();
            MilitaryText = GameTexts.FindText("str_military").ToString();
        }

        [DataSourceMethod]
        public void ExecuteShowMilitary()
        {
            ViewModel!.GetType().GetMethod("SetSelectedCategory", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ViewModel, new object[] { 3 });
        }

        [DataSourceProperty]
        public KingdomMilitaryVM Military
        {
            get
            {
                return VMHelper.Military!;
            }
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
        public string MilitaryText
        {
            get
            {
                return _militaryText;
            }
            set
            {
                if (value != _militaryText)
                {
                    _militaryText = value;
                    OnPropertyChangedWithValue(value, "MilitaryText");
                }
            }
        }
    }
}
