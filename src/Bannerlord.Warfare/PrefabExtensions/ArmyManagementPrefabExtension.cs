using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Bannerlord.Warfare.PrefabExtensions
{
    [PrefabExtension("ArmyManagement", "descendant::ButtonWidget[@IsEnabled='@CanDisbandArmy']")]
    internal sealed class ArmyManagementPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        private readonly XmlDocument _document;

        public ArmyManagementPrefabExtension()
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
}
