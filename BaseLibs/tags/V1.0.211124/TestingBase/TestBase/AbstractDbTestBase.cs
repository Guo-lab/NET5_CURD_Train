using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ProjectBase.Application;
using ProjectBase.Data;
using SharpArch.Domain;
using SharpArch.NHibernate;
using System;
using System.Data;
using System.Data.SQLite;

namespace TestingBase.TestBase
{
    /**
	 *    仿制{@link AbstractTransactionalTestNGSpringContextTests}而写的，包括照抄其操作数据库的方便方法，只是并不将测试方法封装在事务中进行。
	 * spring的{@link AbstractTransactionalTestNGSpringContextTests}类，把每个测试方法都装在一个事务中进行，通过{@link TransactionManager}和{@link TestTransaction}管理事务，
	 * 我们的框架通过{@link HibernateTransaction}管理事务，在测试方法中自定义事务，因此不使用{@link AbstractTransactionalTestNGSpringContextTests}，而使用本类作为基类。
	 * 否则，如果在AbstractTransactionalTestNGSpringContextTests的测试方法中运行自定义事务，就会造成两个事务之间数据读取不同步的问题。  
	 * @author Rainy
	 * @see --internal
	 */
    public class AbstractDbTestBase
    {
        public static string DataFileRoot { get; set; } = "db";

        public static void SetUpTestDB(IAppCommonSetup setup, string dataFileRoot = "db", params string[] sharedDataFiles)
        {
            Check.Require(setup.WindsorContainer != null, "参数setup的属性WindsorContainer不可为空");

            var cfg = setup.InitializeNHibnerate(new SimpleSessionStorage());
            var connection = NHibernateSession.Current.Connection;
            new SchemaExport(cfg).Execute(false, true, false, connection, null);

            SetDataSource(connection);

            DataFileRoot = dataFileRoot;
            if (sharedDataFiles == null) return;
            foreach (var path in sharedDataFiles)
            {
                LoadExcelData(path);
            }
        }

        protected static IDbConnection conn;
        /**
         * The IDbCommand that this base class manages, available to subclasses.
        */
        protected static IDbCommand sqlc;

        private string sqlScriptEncoding;

        public static void LoadExcelData(string filePath)
        {
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            IToSqliteMemory excelToSqlite = new ExcelToSqlite(path + DataFileRoot+ "\\" + filePath);
            excelToSqlite.ToSqliteMemory();
        }
        public static void LoadSqlData(params string[] tableNames)
        {
            IToSqliteMemory sqlServerToSqlite = new SqlServerToSqlite(tableNames);
            sqlServerToSqlite.ToSqliteMemory();
        }

        /**
         * Set the {@code DataSource}, typically provided via Dependency Injection.
         * <p>This method also instantiates the {@link #jdbcTemplate} instance variable.
         */
        public static void SetDataSource(IDbConnection connection)
        {
            conn = connection;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            sqlc = conn.CreateCommand();
        }
        public static IDbConnection GetDataSource()
        {
            return conn;
        }
        public static void CloseTestDB()
        {
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            catch (Exception) { }
        }
        /**
         * Specify the encoding for SQL scripts, if different from the platform encoding.
         * @see #executeSqlScript
         */
        public void SetSqlScriptEncoding(string sqlScriptEncoding)
        {
            this.sqlScriptEncoding = sqlScriptEncoding;
        }

        /**
         * Convenience method for counting the rows in the given table.
         * @param tableName table name to count rows in
         * @return the number of rows in the table
         * @see JdbcTestUtils#countRowsInTable
         */
        protected int CountRowsInTable(string tableName)
        {
            sqlc.CommandText = "select count(*) from " + tableName;
            return (int)(long)sqlc.ExecuteScalar();
        }

        /**
         * Convenience method for counting the rows in the given table, using the
         * provided {@code WHERE} clause.
         * <p>See the Javadoc for {@link JdbcTestUtils#countRowsInTableWhere} for details.
         * @param tableName the name of the table to count rows in
         * @param whereClause the {@code WHERE} clause to append to the query
         * @return the number of rows in the table that match the provided
         * {@code WHERE} clause
         * @since 3.2
         * @see JdbcTestUtils#countRowsInTableWhere
         */
        protected int CountRowsInTableWhere(string tableName, string whereClause)
        {
            sqlc.CommandText = "select count(*) from " + tableName + " where " + whereClause;
            return (int)(long)sqlc.ExecuteScalar();
        }

        /**
         * Convenience method for deleting all rows from the specified tables.
         * <p>Use with caution outside of a transaction!
         * @param names the names of the tables from which to delete
         * @return the total number of rows deleted from all specified tables
         * @see JdbcTestUtils#deleteFromTables
         */
        protected int DeleteFromTables(params string[] names)
        {
            int c = 0;
            foreach (string name in names)
            {
                sqlc.CommandText = "delete from "+ name;
                c+=sqlc.ExecuteNonQuery();
            }
            return c;
        }

        /**
         * Convenience method for deleting all rows from the given table, using the
         * provided {@code WHERE} clause.
         * <p>Use with caution outside of a transaction!
         * <p>See the Javadoc for {@link JdbcTestUtils#deleteFromTableWhere} for details.
         * @param tableName the name of the table to delete rows from
         * @param whereClause the {@code WHERE} clause to append to the query
         * @param args arguments to bind to the query (leaving it to the {@code
         * PreparedStatement} to guess the corresponding SQL type); may also contain
         * {@link org.springframework.jdbc.core.SqlParameterValue SqlParameterValue}
         * objects which indicate not only the argument value but also the SQL type
         * and optionally the scale.
         * @return the number of rows deleted from the table
         * @since 4.0
         * @see JdbcTestUtils#deleteFromTableWhere
         */
        protected int DeleteFromTableWhere(string tableName, string whereClause, params object[] args)
        {
            sqlc.CommandText = "delete from "+ tableName+" where "+ whereClause;
            foreach(object p in args)
            {
                var param = sqlc.CreateParameter();
                param.Value = p;
                sqlc.Parameters.Add(param);
            }
            return sqlc.ExecuteNonQuery();
        }

        /**
         * Convenience method for dropping all of the specified tables.
         * <p>Use with caution outside of a transaction!
         * @param names the names of the tables to drop
         * @since 3.2
         * @see JdbcTestUtils#dropTables
         */
        protected void DropTables(params string[] names)
        {
            foreach (string name in names)
            {
                sqlc.CommandText = "drop table " + name;
                sqlc.ExecuteNonQuery();
            }
        }

        /**
         * Execute the given SQL script.
         * <p>Use with caution outside of a transaction!
         * <p>The script will normally be loaded by classpath.
         * <p><b>Do not use this method to execute DDL if you expect rollback.</b>
         * @param sqlResourcePath the Spring resource path for the SQL script
         * @param continueOnError whether or not to continue without throwing an
         * exception in the event of an error
         * @throws DataAccessException if there is an error executing a statement
         * @see ResourceDatabasePopulator
         * @see #setSqlScriptEncoding
         */
        protected void ExecuteSql(string sql, bool continueOnError)
        {
            sqlc.CommandText = sql;
            sqlc.ExecuteNonQuery();
        }
        protected void ExecuteSqlFile(String sqlResourcePath, bool continueOnError)
        {
            //Files
            sqlc.CommandText = "";
            sqlc.ExecuteNonQuery();
        }

    }
}