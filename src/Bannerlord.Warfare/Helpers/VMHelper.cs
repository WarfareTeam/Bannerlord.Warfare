using Warfare.ViewModels.Military;
using Warfare.ViewModels.World;

namespace Warfare.Helpers
{
    internal class VMHelper
    {
        private static KingdomMilitaryVM? _military;

        private static KingdomWorldVM? _world;

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

        internal static KingdomWorldVM World
        {
            get
            {
                if (_world == null)
                {
                    return _world = new KingdomWorldVM();
                }
                return _world;
            }
            set
            {
                if (value != _world)
                {
                    _world = value;
                }
            }
        }
    }
}
