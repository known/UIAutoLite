using System.Collections.Generic;

namespace UIAutoLite.Data
{
    public class NoneDataSource : DataSource
    {
        public NoneDataSource(string fileName) : base(DataSourceType.None, fileName)
        {
            DataSummary = "无外部数据源！";
        }

        public override List<DataItem> GetDataItems(string tableName, string filterExpression)
        {
            return new List<DataItem>();
        }
    }
}
