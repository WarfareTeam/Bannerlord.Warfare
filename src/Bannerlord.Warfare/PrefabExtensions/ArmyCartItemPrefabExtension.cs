using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("ArmyCartItem", "descendant::TextWidget[@IsHidden='@IsAlreadyWithPlayer']")]
    internal class ArmyCartItemCostPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmyCartItemCostPrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
                <TextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' VerticalAlignment='Center' Brush='ArmyManagement.Army.Cart.Value' Brush.FontSize='24' MarginTop='5' Text='@CostText' IsHidden='@IsAlreadyWithPlayer' />");
        }
        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
    [PrefabExtension("ArmyCartItem", "descendant::ListPanel[@IsHidden='@IsTransferDisabled']")]
    internal class ArmyCartItemSpritePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmyCartItemSpritePrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
                <!--Party Draft Cost Widget-->
                <ListPanel WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' IsHidden='@IsTransferDisabled' HorizontalAlignment='Center'>
                  <Children>
                    <RichTextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' Brush='ArmyManagement.Army.Cart.Value' Brush.FontSize='18' IsVisible='@IsAlreadyWithPlayer' Text='@InArmyText' />
                    <TextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' VerticalAlignment='Center' Brush='ArmyManagement.Army.Cart.Value' Brush.FontSize='24' MarginTop='5' Text='@CostText' IsHidden='@IsAlreadyWithPlayer' />
                    <!--Influence Cost-->
				    <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='30' SuggestedHeight='30' PositionXOffset='-8' IsVisible='@IsInfluenceCost' >
				      <Children>
						<Widget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' Sprite='General\Icons\Influence@2x' MarginLeft='3' IsHidden='@IsAlreadyWithPlayer' />
				      </Children>
				    </Widget>
				    <!--Gold Cost-->
				    <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='30' SuggestedHeight='30' PositionXOffset='-2' PositionYOffset='3' IsHidden='@IsInfluenceCost' >
				      <Children>
						<Widget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' Sprite='General\Icons\Coin@2x' MarginLeft='3' IsHidden='@IsAlreadyWithPlayer' />
				      </Children>
				    </Widget>
                  </Children>
                </ListPanel>");
        }
        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
}