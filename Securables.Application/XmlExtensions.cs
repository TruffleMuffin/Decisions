using System.Xml;
using System.Xml.Linq;

namespace Securables.Application
{
    internal static class XmlExtensions
    {
        public static XElement ToXElement(this XmlNode node)
        {
            var xDoc = new XDocument();
            using (var xmlWriter = xDoc.CreateWriter())
            {
                node.WriteTo(xmlWriter);
            }

            return xDoc.Root;
        }

        public static XmlNode ToXmlNode(this XElement element)
        {
            using (var xmlReader = element.CreateReader())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
    }
}