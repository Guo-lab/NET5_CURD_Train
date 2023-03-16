using Aspose.Cells;
using System.Data;
using System.Data.OleDb;

namespace TestingBase.TestBase
{
    public class ExcelToSqlite : ToSqlMemoryBase, IToSqliteMemory
    {
        private string _sourceFile;
        public ExcelToSqlite(string sourceFile)
        {
            _sourceFile = sourceFile;

        }
        public void ToSqliteMemory()
        {
            string connectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1""", _sourceFile);
            OleDbConnection conn = new OleDbConnection(connectionString);
            Workbook workBook = new Workbook(_sourceFile);

            DataSet ds = new DataSet();

            foreach (Worksheet workSheet in workBook.Worksheets)
            {
                string sql = string.Format("Select * FROM [{0}$]", workSheet.Name);
                OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                adapter.Fill(ds, workSheet.Name);
            }


            CopyFromDataSet(ds,true);

        }
    }
}
