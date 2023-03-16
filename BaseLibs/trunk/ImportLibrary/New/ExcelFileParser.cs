using Aspose.Cells;
using ProjectBase.Web.Mvc.ValueInFile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImportLibrary.New
{
    public class ExcelFileParser : ImportLibrary.ExcelFileParser, IValueInFileParser
    {
        protected virtual IList<string> ColNames { get; }//TODO：可配置从第一行取

        /// <summary>
        /// 构成表格行列，一行对应一条记录的，起始行.之前的行由子类自行处理
        /// </summary>
        protected virtual int TableRowFromIndex { get; }

        protected virtual IList<Dictionary<string, string>> Sort(IList<Dictionary<string, string>> parsedRows)
        {
            return parsedRows;
        }
        public virtual IList<Dictionary<string, string>> ParseAsNV(FileInfo file)
        {
            var nvlist = new List<Dictionary<string, string>>();

            var workBook = new Workbook(file.FullName);
            var cells = workBook.Worksheets[0].Cells;

            for (var rowIndex = TableRowFromIndex; rowIndex < cells.Rows.Count; rowIndex++)
            {
                if (!string.IsNullOrEmpty(GetCellValue(cells, rowIndex, 0))) continue;//第一列空则此行不处理

                var valuesInRow = new Dictionary<string, string>();
                for (var colIndex = 0; colIndex < ColNames.Count; colIndex++)
                {
                    valuesInRow[ColNames[colIndex]] = GetCellValue(cells, rowIndex, colIndex);
                }
                nvlist.Add(valuesInRow);
            }
            return Sort(nvlist);
        }

    }
}
