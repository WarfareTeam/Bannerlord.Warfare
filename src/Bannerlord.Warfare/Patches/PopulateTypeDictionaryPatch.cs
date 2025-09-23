using System;
using System.Collections.Generic;

using TaleWorlds.CampaignSystem.ViewModelCollection.Map;

using HarmonyLib;

using Warfare.Notifications;

namespace Warfare.Patches
{
    [HarmonyPatch(typeof(MapNotificationVM), "PopulateTypeDictionary")]
    public static class PopulateTypeDictionaryPatch
    {
        public static void Postfix(ref Dictionary<Type, Type> ____itemConstructors)
        {
            ____itemConstructors.Add(typeof(VillageRaidedMapNotification), typeof(VillageRaidedMapNotificationItemVM));
        }
    }
}
