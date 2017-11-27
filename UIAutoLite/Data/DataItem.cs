using System.Data;

namespace UIAutoLite.Data
{
    public abstract class DataItem
    {
        public string GetValue(string switchType, string columnName)
        {
            var value = GetValue(columnName);
            var switchData = DataSource.SwitchData;
            if (!string.IsNullOrEmpty(switchType) && switchData != null && switchData.Rows.Count > 0)
            {
                var datas = switchData.Select("SWITCH_TYPE='" + switchType + "'");
                if (datas != null && datas.Length > 0)
                {
                    foreach (var data in datas)
                    {
                        if (data.Field<string>("SOURCE_VALUE") == value)
                        {
                            value = data.Field<string>("TARGET_VALUE");
                            break;
                        }
                    }
                }
            }

            return value;
        }

        public abstract string GetValue(string columnName);
    }
}
