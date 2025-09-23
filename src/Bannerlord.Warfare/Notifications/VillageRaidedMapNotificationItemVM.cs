using System;

using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;
using TaleWorlds.Library;

namespace Warfare.Notifications
{
    public class VillageRaidedMapNotificationItemVM : MapNotificationItemBaseVM
    {
        private Village _village;

        public VillageRaidedMapNotificationItemVM(VillageRaidedMapNotification data) : base(data)
        {
            _village = data.Village;
            NotificationIdentifier = "settlementundersiege";
            _onInspect = delegate ()
            {
                Action<Vec2> fastMoveCameraToPosition = FastMoveCameraToPosition;
                if (fastMoveCameraToPosition == null)
                {
                    return;
                }
                fastMoveCameraToPosition(_village.Settlement.Position2D);
            };
        }
    }
}
