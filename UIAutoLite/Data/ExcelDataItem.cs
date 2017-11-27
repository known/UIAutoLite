using System.Data;

namespace UIAutoLite.Data
{
    public class ExcelDataItem : DataItem
    {
        DataRow dataRow = null;

        public ExcelDataItem(DataRow dataRow)
        {
            this.dataRow = dataRow;
        }

        public override string GetValue(string columnName)
        {
            if (dataRow == null || string.IsNullOrEmpty(columnName))
                return string.Empty;

            if (dataRow.Table.Columns.Contains(columnName))
                return dataRow.Field<string>(columnName);

            return string.Empty;
        }
    }
}
