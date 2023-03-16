using ProjectBase.Web.Mvc.ValueInFile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImportLibrary.New
{
    public class CSVFileParser : IValueInFileParser
    {
        protected virtual char Delimeter { get; set; } = '|';
        protected virtual IList<string> ColNames { get;}

        protected virtual IList<Dictionary<string, string>> Sort(IList<Dictionary<string, string>> parsedRows)
        {
            return parsedRows;
        }
        public virtual IList<Dictionary<string, string>> ParseAsNV(FileInfo file)
        {
            var nvlist = new List<Dictionary<string, string>>();

            StreamReader reader = new StreamReader(file.FullName, Encoding.UTF8);
            string? line = reader.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                string[] row = line.Split(Delimeter);
                if (row.Length == ColNames.Count)
                {
                    var valuesInRow = new Dictionary<string, string>();
                    for (var colIndex=0;colIndex<ColNames.Count;colIndex++)
                    {
                        valuesInRow[ColNames[colIndex]] = row[colIndex];
                    }
                    line = reader.ReadLine();
                    nvlist.Add(valuesInRow);
                }
                else
                {
                    line = reader.ReadLine();
                }
            }
            reader.Close();
            return Sort(nvlist);
        }


    }
}
