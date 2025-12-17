using System.Collections.Generic;
using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("ArmyManagementRightPanel", "descendant::ButtonWidget[@Id='DisbandButton']")]
    internal class ArmyManagementDisbandArmyButtonPrefabExtension : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("IsHidden", "@IsSplitArmy")
        };
    }
    [PrefabExtension("ArmyManagementRightPanel", "descendant::ButtonWidget[@IsEnabled='@CanDisbandArmy']")]
    internal sealed class ArmyManagementDisbandCostPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        private readonly XmlDocument _document;

        public ArmyManagementDisbandCostPrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
				<ListPanel WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' HorizontalAlignment='Center' MarginTop='60' IsHidden='@IsDisbandWithCost'>
				  <Children>
					<TextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' VerticalAlignment='Center' Brush='Kingdom.GeneralButtons.Text' IntText='@DisbandCost' IsEnabled='@CanDisbandArmy'>
						<Children>
							<Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='29' SuggestedHeight='29' MarginRight='-24' HorizontalAlignment='Right' VerticalAlignment='Center' Sprite='General\Icons\Influence@2x'/>
						</Children>
					</TextWidget>
				  </Children>
				</ListPanel>");
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
    [PrefabExtension("ArmyManagement", "descendant::ListPanel[@SuggestedHeight='70']")]
    internal sealed class ArmyManagementTotalCostPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmyManagementTotalCostPrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
                <ListPanel WidthSizePolicy='CoverChildren' HeightSizePolicy='Fixed' SuggestedHeight='70' HorizontalAlignment='Center' VerticalAlignment='Bottom' MarginBottom='30' StackLayout.LayoutMethod='VerticalBottomToTop' >
                  <Children>
                    <!--Total Cost Text-->
                    <RichTextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' Brush='ArmyManagement.Army.TotalCostAndStrength.Title' Text='@TotalCostText' />

                    <!--Affordable Gold Cost-->
				    <Widget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='12' IsVisible='@CanAffordGoldCost'>
				      <Children>
					    <RichTextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='-24' Brush='WarfareArmyManagement.Army.TotalCostAndStrength.PositiveValue' Text='@TotalGoldCostNumbersText' />
				        <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='27' SuggestedHeight='27' HorizontalAlignment='Right' VerticalAlignment='Center' Sprite='General\Icons\Coin@2x'/>
				      </Children>
				    </Widget>
				
                    <!--Unaffordable Gold Cost-->
				    <Widget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='12' IsHidden='@CanAffordGoldCost'>
				      <Children>
					    <RichTextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='-24' Brush='WarfareArmyManagement.Army.TotalCostAndStrength.NegativeValue' Text='@TotalGoldCostNumbersText' />
				        <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='27' SuggestedHeight='27' HorizontalAlignment='Right' VerticalAlignment='Center' Sprite='General\Icons\Coin@2x'/>
				      </Children>
				    </Widget>

                    <!--Affordable Influence Cost-->
				    <Widget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='12' IsVisible='@CanAffordInfluenceCost'>
				      <Children>
					    <RichTextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='-24' Brush='WarfareArmyManagement.Army.TotalCostAndStrength.PositiveValue' Text='@TotalInfluenceCostNumbersText' />
				        <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='27' SuggestedHeight='27' HorizontalAlignment='Right' VerticalAlignment='Center' Sprite='General\Icons\Influence@2x'/>
				      </Children>
				    </Widget>

                    <!--Unaffordable Influence Cost-->
				    <Widget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='12' IsHidden='@CanAffordInfluenceCost'>
				      <Children>
					    <RichTextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='StretchToParent' HorizontalAlignment='Center' PositionXOffset='-24' Brush='WarfareArmyManagement.Army.TotalCostAndStrength.PositiveValue' Text='@TotalInfluenceCostNumbersText' />
				        <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='27' SuggestedHeight='27' HorizontalAlignment='Right' VerticalAlignment='Center' Sprite='General\Icons\Influence@2x'/>
				      </Children>
				    </Widget>
                  </Children>
                </ListPanel>");
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
    [PrefabExtension("ArmyManagementLeftPanel", "descendant::Constant[@Name='ArmyManagement.Panel.Left.Width']")]
    internal sealed class ArmyManagementLeftPanelAdditivePatch : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("Additive", "107")
        };
    }
    [PrefabExtension("ArmyManagementLeftPanel", "descendant::Constant[@Name='ArmyManagement.Sort.5.Width']")]
    internal sealed class ArmyManagementLeftPanelSortAdditivePatch : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new()
        {
            new Attribute("Additive", "28")
        };
    }
    [PrefabExtension("ArmyManagementLeftPanel", "descendant::BrushWidget[@Id='GatherArmyPartiesPanel']")]
    internal sealed class ArmyManagementLeftPanelPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmyManagementLeftPanelPrefabExtension()
        {
            _document = new XmlDocument();
            if (SubModule.NavalDLC)
            {
                _document.LoadXml(@"<NavalArmyManagementLeftPanelCustom />");
            }
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
}
