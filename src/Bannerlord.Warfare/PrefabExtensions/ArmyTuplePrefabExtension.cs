using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Warfare.PrefabExtensions
{
    /*[PrefabExtension("ArmyTuple", "descendant::ListPanel[@DoNotAcceptEvents='true']")]
    internal class ArmyTuplePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmyTuplePrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"
                <ListPanel HeightSizePolicy ='StretchToParent' WidthSizePolicy='StretchToParent' DoNotAcceptEvents='true'>
                  <Children>
                    <!--Leader Visual-->
                    <ButtonWidget DataSource='{Leader}' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='112' SuggestedHeight='81' VerticalAlignment='Center' Command.Click='ExecuteLink' IsEnabled='false'>
                      <Children>
                        <MaskedTextureWidget DataSource='{ClanBanner_9}' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='!Banner.Width.Scaled' SuggestedHeight='!Banner.Height.Scaled' HorizontalAlignment='Right' VerticalAlignment='Top' PositionYOffset='-4' Brush='Flat.Tuple.Banner.Small.Hero' AdditionalArgs='@AdditionalArgs' ImageId='@Id' TextureProviderName='@TextureProviderName' IsDisabled='true'  />
                        <ImageIdentifierWidget DataSource='{ImageIdentifier}' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='100' SuggestedHeight='74' HorizontalAlignment='Center' VerticalAlignment='Center' AdditionalArgs='@AdditionalArgs' ImageId='@Id' TextureProviderName='@TextureProviderName' IsEnabled='false'/>
                        <HintWidget Command.HoverBegin='ExecuteBeginHint' Command.HoverEnd='ExecuteEndHint' WidthSizePolicy = 'StretchToParent' HeightSizePolicy = 'StretchToParent' IsEnabled='false'/>
                      </Children>
                    </ButtonWidget>

                    <!--Army Name-->
			        <RichTextWidget DoNotAcceptEvents='true' WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' VerticalAlignment='Center' Brush='ArmyManagement.Army.Tuple.Name' IsEnabled='false' Text='@ArmyName' />

                    <!--War Icon-->
                    <Widget DoNotAcceptEvents='true' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='63' SuggestedHeight='63' HorizontalAlignment='Center' MarginRight='20' MarginTop='10' Sprite='SPKingdom\Diplomacy\diplomacy_war_icon' IsEnabled='false' />
                  </Children>
                </ListPanel>");
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }*/
    [PrefabExtension("ArmyTuple", "descendant::ButtonWidget[@IsSelected='@IsSelected']")]
    internal class ArmyTuplePrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmyTuplePrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"<ArmyTupleCustom />");
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;
    }
}
