using System.Xml;

using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Warfare.PrefabExtensions
{
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
