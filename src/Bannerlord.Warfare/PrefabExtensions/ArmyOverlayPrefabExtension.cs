using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using System.Collections.Generic;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("ArmyOverlay", "descendant::ButtonWidget[@Id='ArmyOverlayArmyManagementButton']")]
    internal class ArmyOverlayArmyManagementButtonPrefabExtension : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("IsVisible", "true"),
            new Attribute("IsEnabled", "true"),
        };
    }
    [PrefabExtension("ArmyOverlay", "descendant::ButtonWidget[@GamepadNavigationIndex='1']")]
    internal class ArmyOverlayArmyCohesionPrefabExtension : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("IsEnabled", "true")
        };
    }
}