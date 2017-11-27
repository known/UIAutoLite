using System.Collections.Generic;

namespace UIAutoLite.Data
{
    public class YunDataSource : DataSource
    {
        public YunDataSource(string fileName) : base(DataSourceType.Yun, fileName)
        {
            DataSummary = "云数据源！";
        }

        public override List<DataItem> GetDataItems(string tableName, string filterExpression)
        {
            return new List<DataItem>();
        }
    }
}
