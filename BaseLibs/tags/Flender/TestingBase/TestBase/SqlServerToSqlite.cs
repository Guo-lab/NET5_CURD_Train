using Aspose.Cells;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace TestingBase.TestBase
{
    /// <summary>
    /// 从sqlserver复制数据到内存数据库
    /// </summary>
    public class SqlServerToSqlite : ToSqlMemoryBase, IToSqliteMemory
    {
        private string[] tableNames;
        public SqlServerToSqlite(params string[] tableNames)
        {
            this.tableNames = tableNames;
        }
        public void ToSqliteMemory()
        {
            var str = TestingUtil.GetToolSetting("ConnectionString");
             SqlConnection conn = new SqlConnection(str);
            DataSet ds = new DataSet();

            foreach (var tbl in tableNames)
            {
                string sql = "Select * FROM "+ tbl;
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                adapter.Fill(ds, tbl);
            }

            CopyFromDataSet(ds,false);
        }
    }
}
