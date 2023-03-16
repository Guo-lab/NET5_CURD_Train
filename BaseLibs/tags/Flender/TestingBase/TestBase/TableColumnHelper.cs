using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace TestingBase.TestBase
{
    public static class TableColumnHelper
    {
        private class ColumnInfo
        {
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public DbType DataType { get; set; }
        }
        private static IList<ColumnInfo> _columnDataType;

        static TableColumnHelper()
        {
            _columnDataType = new List<ColumnInfo>();
            SqlConnection conn = new SqlConnection(TestingUtil.GetToolSetting("ConnectionString"));
            //SqlConnection conn = new SqlConnection("Data Source = 114.115.147.8,1566; Database = Test_P2104; User ID = sa; Password = Jmei34hs19gk");
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Select TABLE_NAME,COLUMN_NAME, DATA_TYPE from INFORMATION_SCHEMA.COLUMNS";
            cmd.CommandType = CommandType.Text;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _columnDataType.Add(new ColumnInfo
                {
                    TableName = reader["TABLE_Name"].ToString(),
                    ColumnName = reader["COLUMN_NAME"].ToString(),
                    DataType = GetDataType(reader["DATA_TYPE"].ToString())
                });
            }
        }

        public static DbType GetDataType(string tableName, string columnName)
        {
            ColumnInfo info = _columnDataType.FirstOrDefault(x => x.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && x.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            if (info != null)
            {
                return info.DataType;
            }
            else
            {
                throw new Exception("Invalid Table/Column Name:" + tableName + "." + columnName);
            }
        }

        private static DbType GetDataType(string type)
        {
            switch (type)
            {
                case "nvarchar":
                case "varchar":
                case "nchar":
                case "char":
                case "ntext":
                case "text":
                    return DbType.String;
                case "datetime":
                    return DbType.DateTime;
                case "int":
                case "smallint":
                case "bigint":
                    return DbType.Int32;
                case "decimal":
                    return DbType.Decimal;
                case "bit":
                    return DbType.Boolean;
                case "uniqueidentifier":
                    return DbType.Guid;
                default:
                    return DbType.String;
            }
        }

        public static object ConvertToType(this object obj, DbType type)
        {
            switch (type)
            {
                case DbType.String:
                    return obj.ToString();
                case DbType.DateTime:
                    return Convert.ToDateTime(obj);
                case DbType.Int32:
                    return Convert.ToInt32(obj);
                case DbType.Decimal:
                    return Convert.ToDecimal(obj);
                case DbType.Boolean:
                    return Convert.ToBoolean(obj);
                case DbType.Guid:
                    return new Guid(obj.ToString());
                default:
                    return obj.ToString();
            }
        }
    }
}
