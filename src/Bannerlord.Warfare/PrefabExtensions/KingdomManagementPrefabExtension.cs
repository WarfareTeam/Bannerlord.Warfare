using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Xml;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("KingdomManagement", "descendant::Constant[@Name='Header.Tab.Center.Width.Scaled']")]
    [UsedImplicitly]
    internal sealed class KingdomManagementScalingPatch : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("MultiplyResult", "0.60")
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
    [PrefabExtension("KingdomManagement", "descendant::ButtonWidget[@Id='DiplomacyTabButton']")]
    internal sealed class KingdomManagementDiplomacyTabPrefabExtension : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("MarginLeft", "5"),
            new Attribute("MarginLeft", "5"),
            new Attribute("SuggestedWidth", "!Header.Tab.Center.Width.Scaled"),
            new Attribute("SuggestedHeight", "!Header.Tab.Center.Height.Scaled"),
            new Attribute("PositionYOffset", "2"),
            new Attribute("Position", "5"),
            new Attribute("MarginLeft", "5"),
            new Attribute("Brush", "Header.Tab.Center"),
        };
    }
    [PrefabExtension("KingdomManagement", "descendant::KingdomTabControlListPanel")]
    internal sealed class KingdomManagementKingdomTabControlListPanelPrefabExtension : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("WorldButton", "WorldTabButton"),
            new Attribute("WorldPanel", "..\\..\\WorldPanel")
        };
    }
    [PrefabExtension("KingdomManagement", "descendant::KingdomTabControlListPanel/Children")]
    internal sealed class KingdomManagementWorldTabButtonPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Child;
        public override int Index => 5;
        private readonly XmlDocument _document;
        public KingdomManagementWorldTabButtonPrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
                <!--World Button-->
                <ButtonWidget Id='WorldTabButton' DoNotPassEventsToChildren='true' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='!Header.Tab.Right.Width.Scaled' SuggestedHeight='!Header.Tab.Right.Height.Scaled' HorizontalAlignment='Right' VerticalAlignment='Center' MarginLeft='5' Brush='Header.Tab.Right' Command.Click='ExecuteShowWorld' UpdateChildrenStates='true'>
                  <Children>
                    <TextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' HorizontalAlignment='Center' VerticalAlignment='Center' Brush='Clan.TabControl.Text' Text='@WorldText' />
                  </Children>
                </ButtonWidget>");
        }
        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
    [PrefabExtension("KingdomManagement", "descendant::ArmiesPanel[@Id='ArmiesPanel']")]
    internal sealed class KingdomManagementArmiesPanelPrefabExtension : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("DataSource", "{Military}")
        };
    }
    [PrefabExtension("KingdomManagement", "descendant::DiplomacyPanel")]
    internal sealed class KingdomManagementWorldTabDataPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;
        private readonly XmlDocument _document;
        public KingdomManagementWorldTabDataPrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"<WorldPanel Id='WorldPanel' DataSource='{World}' MarginTop='188' MarginBottom='75' />");
        }
        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
}
