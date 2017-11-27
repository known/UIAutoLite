using System.Xml;

namespace UIAutoLite.Data
{
    public class XmlDataItem : DataItem
    {
        XmlNode xmlNode = null;
        XmlNamespaceManager nsmgr = null;

        public XmlDataItem(XmlNode xmlNode, XmlNamespaceManager nsmgr)
        {
            this.xmlNode = xmlNode;
            this.nsmgr = nsmgr;
        }

        public override string GetValue(string columnName)
        {
            var xpath = columnName;
            if (xmlNode == null || string.IsNullOrEmpty(xpath))
                return string.Empty;

            var node = nsmgr != null ? xmlNode.SelectSingleNode(xpath, nsmgr) : xmlNode.SelectSingleNode(xpath);
            if (node != null)
                return node.InnerText;

            return string.Empty;
        }
    }
}
