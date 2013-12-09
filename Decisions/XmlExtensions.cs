using System.Xml;
using System.Xml.Linq;

namespace Decisions
{
    /// <summary>
    /// Some extensions to <see cref="XElement"/> for use within Decisions
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Converst the specified <see cref="XElement"/> to a <see cref="XmlNode"/>
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>An <see cref="XmlNode"/></returns>
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