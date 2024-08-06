using ClosedXML.Excel;

namespace manto_stock_system_API.Extensions
{
    public static class XLWorkbookExtensions
    {
        public static string ToBase64(this XLWorkbook workbook)
        {
            using MemoryStream stream = new();
            workbook.SaveAs(stream);
            return Convert.ToBase64String(stream.ToArray());
        }
    }
}
