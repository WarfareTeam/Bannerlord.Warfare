using Warfare.ViewModels.Military;

namespace Warfare.Helpers
{
    internal class VMHelper
    {
        private static KingdomMilitaryVM? _military;

        internal static KingdomMilitaryVM Military
        {
            get
            {
                if (_military == null)
                {
                    return _military = new KingdomMilitaryVM();
                }
                return _military;
            }
            set
            {
                if (value != _military)
                {
                    _military = value;
                }
            }
        }
    }
}
