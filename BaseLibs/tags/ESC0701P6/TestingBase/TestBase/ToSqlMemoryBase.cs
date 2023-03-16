using NHibernate;
using ProjectBase.Data;
using SharpArch.NHibernate;
using System.Data;
using System.Text;
namespace TestingBase.TestBase
{
    public class ToSqlMemoryBase
    {
        //convertNulltoEmptyString=true Excel中的空白单元格读入DataSet会转换成DBNULL,需要将其转为空白字符串
        //真正的null值必须保留NULL字样
        protected void CopyFromDataSet(DataSet dataset, bool convertNulltoEmptyString)
        {
            IDbConnection conn = AbstractDbTestBase.GetDataSource();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA foreign_keys = OFF";
            cmd.ExecuteNonQuery();

            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                foreach (DataTable table in dataset.Tables)
                {
                    using (var command = conn.CreateCommand())
                    {
                        StringBuilder sbInsert = new StringBuilder("Insert into " + table.TableName + " (");
                        StringBuilder sbValues = new StringBuilder();
                        foreach (DataColumn column in table.Columns)
                        {
                            sbInsert.Append(column.ColumnName + ",");
                            sbValues.Append("@" + column.ColumnName + ",");

                            IDbDataParameter parameter = command.CreateParameter();
                            parameter.ParameterName = "@" + column.ColumnName;
                            parameter.DbType = TableColumnHelper.GetDataType(table.TableName, column.ColumnName);
                            command.Parameters.Add(parameter);
                        }
                        string insert = sbInsert.ToString();
                        string values = sbValues.ToString();
                        command.CommandText = insert.Substring(0, insert.Length - 1) + ") values(" + values.Substring(0, values.Length - 1) + ")";

                        foreach (DataRow row in table.Rows)
                        {
                            for (int i = 0; i < command.Parameters.Count; i++)
                            {
                                var p = ((IDbDataParameter)command.Parameters[i]);
                                var isStringParam = p.DbType == DbType.AnsiString || p.DbType == DbType.AnsiStringFixedLength || p.DbType == DbType.String || p.DbType == DbType.StringFixedLength;
                                object value = row[i];
                                if (value == System.DBNull.Value ) 
                                {
                                    if (convertNulltoEmptyString)
                                    {
                                        value = "";
                                    }
                                    else
                                    {
                                        value = null;
                                    }
                                }
                                else if (value.ToString().ToUpper() == "NULL")
                                {
                                    value = null;
                                }
                                if (value == null)
                                {
                                    p.Value = value;
                                }
                                else
                                {
                                    p.Value = value.ConvertToType(p.DbType);
                                }
                            }
                            command.ExecuteNonQuery();
                        }
                    }
                }

                //cmd.CommandText = "PRAGMA foreign_keys=ON";
                //cmd.ExecuteNonQuery();
                transaction.Commit();
            }
        }
    }
}
