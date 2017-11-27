using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace UIAutoLite.Data
{
    public class XmlDataSource : DataSource
    {
        XmlDocument dataSource = null;
        XmlNamespaceManager nsmgr = null;

        public XmlDataSource(string fileName) : base(DataSourceType.Xml, fileName)
        {
            dataSource = new XmlDocument();
            dataSource.Load(fileName);

            if (!dataSource.HasChildNodes)
                throw new Exception("没有需要录入的数据！");

            var rootNode = dataSource.DocumentElement;
            if (!string.IsNullOrEmpty(rootNode.NamespaceURI))
            {
                nsmgr = new XmlNamespaceManager(dataSource.NameTable);
                nsmgr.AddNamespace(rootNode.Prefix, rootNode.NamespaceURI);
            }

            var sb = new StringBuilder();
            sb.AppendLine("XML数据源根节点名称：" + rootNode.Name);
            sb.AppendLine("XML数据源根节点本地名称：" + rootNode.LocalName);
            sb.AppendLine("XML数据源节点前缀：" + rootNode.Prefix);
            sb.AppendLine("XML数据源节点命名空间URI：" + rootNode.NamespaceURI);
            DataSummary = sb.ToString();
        }

        public override List<DataItem> GetDataItems(string tableName, string filterExpression)
        {
            var xpath = tableName;
            if (string.IsNullOrEmpty(xpath))
                return null;

            var nodes = nsmgr != null ? dataSource.SelectNodes(xpath, nsmgr) : dataSource.SelectNodes(xpath);
            if (nodes == null || nodes.Count == 0)
                return null;

            var items = new List<DataItem>();
            foreach (XmlNode node in nodes)
            {
                items.Add(new XmlDataItem(node, nsmgr));
            }
            return items;
        }
    }
}
