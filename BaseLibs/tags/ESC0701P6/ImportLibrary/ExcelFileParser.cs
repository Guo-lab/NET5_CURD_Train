using Aspose.Cells;


namespace ImportLibrary
{
    public class ExcelFileParser
    {
        //从Excel中找到指定行pnRow，指定列pnCol中的信息，没有取到的返回空字符串
        protected string GetCellValue(Cells poCells, int pnRow, int pnCol)
        {
            if (poCells[pnRow, pnCol].Value == null)
            {
                return "";
            }
            else
            {
                return poCells[pnRow, pnCol].Value.ToString().Trim();
            }
        }
    }
}
