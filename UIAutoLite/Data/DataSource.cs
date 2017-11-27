using System.Collections.Generic;
using System.Data;

namespace UIAutoLite.Data
{
    public abstract class DataSource
    {
        public DataSource(DataSourceType dataSourceType, string fileName)
        {
            DataSourceType = dataSourceType;
            FileName = fileName;
        }

        public static DataSource CreateDataSource(DataSourceType dataSourceType, string fileName)
        {
            switch (dataSourceType)
            {
                case DataSourceType.Excel:
                    return new ExcelDataSource(fileName);
                case DataSourceType.Xml:
                    return new XmlDataSource(fileName);
                case DataSourceType.Yun:
                    return new YunDataSource(fileName);
                default:
                    return new NoneDataSource(fileName);
            }
        }

        public static DataTable SwitchData { get; set; }
        public DataSourceType DataSourceType { get; }
        public string FileName { get; }
        public string DataSummary { get; protected set; }
        public string Printer { get; set; }

        public List<DataItem> GetDataItems(string tableName)
        {
            return GetDataItems(tableName, null);
        }

        public abstract List<DataItem> GetDataItems(string tableName, string filterExpression);
    }
}
