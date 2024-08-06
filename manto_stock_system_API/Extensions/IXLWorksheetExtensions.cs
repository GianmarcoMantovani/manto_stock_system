using ClosedXML.Excel;

namespace manto_stock_system_API.Extensions
{
    public static class IXLWorksheetExtensions
    {
        public static void ApplyTableStyle(this IXLWorksheet worksheet)
        {
            worksheet.Columns().AdjustToContents();
            worksheet.Cells().Style.Alignment.WrapText = true;
            worksheet.Cells().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cells().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            foreach (var column in worksheet.Columns()) column.Width += 2; // Padding between columns
            worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed()).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed()).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed()).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed()).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed()).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Range(worksheet.FirstCellUsed(), worksheet.LastCellUsed()).Style.Border.TopBorder = XLBorderStyleValues.Thin;

            worksheet.Style.Font.Bold = true;
        }

        public static void AddColumns(this IXLWorksheet worksheet, string[] columns)
        {
            for (var i = 0; i < columns.Length; ++i) worksheet.Cell(1, i + 1).Value = columns[i];
        }
    }
}
