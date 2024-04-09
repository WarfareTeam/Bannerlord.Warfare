using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Warfare.PrefabExtensions
{
    [PrefabExtension("ArmiesPanel", "descendant::Widget[@IsVisible='@Show']")]
    internal class ArmiesPanelPrefabExtension : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Replace;

        private readonly XmlDocument _document;

        public ArmiesPanelPrefabExtension()
        {
            _document = new XmlDocument();
            _document.LoadXml(@"<ArmiesPanelCustom />");
        }

        [PrefabExtensionXmlDocument]
        public XmlDocument GetPrefabExtension() => _document;

    }
}