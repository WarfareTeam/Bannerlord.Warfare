using System.Collections.Generic;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("KingdomManagement", "descendant::ArmiesPanel[@Id='ArmiesPanel']")]
    internal sealed class KingdomManagementArmiesPanelPrefabExtension : PrefabExtensionSetAttributePatch
    {

        public override List<Attribute> Attributes => new()
        {
            new Attribute("DataSource", "{Military}")
        };
    }
    [PrefabExtension("KingdomManagement", "descendant::ButtonWidget[@Id='ArmiesTabButton']")]
    internal sealed class KingdomManagementArmiesTabButtonPrefabExtension : PrefabExtensionSetAttributePatch
    {

        public override List<Attribute> Attributes => new()
        {
            new Attribute("Command.Click", "ExecuteShowMilitary")
        };
    }
    [PrefabExtension("KingdomManagement", "descendant::ButtonWidget[@Id='ArmiesTabButton']/Children/TextWidget")]
    internal sealed class KingdomManagementArmiesTabTextPrefabExtension : PrefabExtensionSetAttributePatch
    {

        public override List<Attribute> Attributes => new()
        {
            new Attribute("Text", "@MilitaryText")
        };
    }
}
