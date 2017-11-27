using Aspose.Cells;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UIAutoLite.Data
{
    public class ExcelDataSource : DataSource
    {
        public ExcelDataSource(string fileName) : base(DataSourceType.Excel, fileName)
        {
            var wb = new Workbook(fileName);
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("数据源共{0}张表", wb.Worksheets.Count));
            foreach (Worksheet sheet in wb.Worksheets)
            {
                sb.AppendLine(string.Format("表{0}共{1}行", sheet.Name, sheet.Cells.MaxDataRow + 1));
            }
            DataSummary = sb.ToString();
        }

        public override List<DataItem> GetDataItems(string tableName, string filterExpression)
        {
            if (string.IsNullOrEmpty(tableName))
                return null;

            var wb = new Workbook(FileName);
            var sheet = wb.Worksheets[tableName];
            if (sheet == null)
                return null;

            var totalRows = sheet.Cells.MaxDataRow + 1;
            var totalColumns = sheet.Cells.MaxDataColumn + 1;
            var dataTable = sheet.Cells.ExportDataTableAsString(0, 0, totalRows, totalColumns, true);
            if (dataTable == null || dataTable.Rows.Count == 0)
                return null;

            if (!string.IsNullOrEmpty(filterExpression))
            {
                dataTable = dataTable.Select(filterExpression).CopyToDataTable();

                if (dataTable == null || dataTable.Rows.Count == 0)
                    return null;
            }

            var items = new List<DataItem>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                items.Add(new ExcelDataItem(dataRow));
            }
            return items;
        }
    }
}
