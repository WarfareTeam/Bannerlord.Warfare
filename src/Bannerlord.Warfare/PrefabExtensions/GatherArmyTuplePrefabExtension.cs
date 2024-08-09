using System.Collections.Generic;
using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("GatherArmyTuple", "descendant::ListPanel[@WidthSizePolicy='CoverChildren']")]
    internal sealed class GatherArmyTuplePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public GatherArmyTuplePrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
                <ListPanel WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' HorizontalAlignment='Center' VerticalAlignment='Center' MarginRight='-16'>
                  <Children>
	                <TextWidget WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' Brush='ArmyManagement.Army.Tuple.Value' MarginTop='5' Text='@CostText' />
	                <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='34' SuggestedHeight='34' VerticalAlignment='Center' Sprite='General\Icons\Influence@2x' IsVisible='@IsInfluenceCost' />
	                <Widget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='28' SuggestedHeight='28' VerticalAlignment='Center' Sprite='General\Icons\Coin@2x' IsHidden='@IsInfluenceCost' />
                  </Children>
                </ListPanel>");
        }
        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
}