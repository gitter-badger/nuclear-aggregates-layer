using System;
using System.Data;
using System.Data.SqlClient;

using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace DoubleGis.Erm.BL.Reports
{
    /// <summary>
    /// Расширения классов
    /// </summary>
    internal static class Extensions
    {

        #region ExcelColumn

        public static void AutoFit2(this ExcelColumn column, double maxWidth)
        {
            column.AutoFit();
            column.Width = column.Width > maxWidth ? column.Width = maxWidth : column.Width;
        }

        public static void AutoFit2(this ExcelColumn column, double minWidth, double maxWidth)
        {
            column.AutoFit(minWidth);
            column.Width = column.Width > maxWidth ? column.Width = maxWidth : column.Width;
        }

        #endregion

        #region ExcelTable

        public static ExcelRange GetDataRange(this ExcelTable table)
        {
            return
                table.WorkSheet.Cells[
                    table.Address.Start.Row + Convert.ToInt32(table.ShowHeader),
                    table.Address.Start.Column,
                    table.Address.End.Row - Convert.ToInt32(table.ShowTotal),
                    table.Address.End.Column
                    ];
        }

        public static ExcelRange GetColumnDataRange(this ExcelTable table, int columnPosition)
        {
            int columnSheetId = table.Address.Start.Column + columnPosition;
            return
                table.WorkSheet.Cells[
                    table.Address.Start.Row + Convert.ToInt32(table.ShowHeader),
                    columnSheetId,
                    table.Address.End.Row - Convert.ToInt32(table.ShowTotal),
                    columnSheetId
                    ];
        }

        public static void EnableTotals(this ExcelTable table)
        {
            table.ShowTotal = true;

            var totalsRowIndex = table.Address.End.Row;

            foreach (var col in table.Columns)
            {
                var colIndex = table.Address.Start.Column + col.Position;

                col.TotalsRowFunction = RowFunctions.Sum;
                table.WorkSheet.Cells[totalsRowIndex, colIndex].Style.Numberformat.Format =
                    table.WorkSheet.Cells[totalsRowIndex - 1, colIndex].Style.Numberformat.Format;
            }
        }

        public static ExcelRange GetHeaderDataRange(this ExcelTable table)
        {
            if (!table.ShowHeader)
                throw new InvalidOperationException("Header is hidden");

            return
                table.WorkSheet.Cells[
                    table.Address.Start.Row,
                    table.Address.Start.Column,
                    table.Address.Start.Row,
                    table.Address.End.Column
                    ];
        }

        #endregion

        #region ExcelRange

        public static ExcelRange GetColumnRange(this ExcelRange range, int columnNumber)
        {
            return range.Worksheet.Cells[
                range.Offset(
                    0,
                    columnNumber - 1,
                    range.End.Row - range.Start.Row + 1,
                    1
                    ).Address
                ];
        }

        public static bool Intersecting(this ExcelRange thisRange, ExcelRange otherRange)
        {
            if (thisRange == null || otherRange == null)
                return false;
            if (thisRange.Worksheet != otherRange.Worksheet)
                return false;

            return (
                    thisRange.Start.Column <= otherRange.End.Column && otherRange.Start.Column <= thisRange.End.Column
                    && thisRange.Start.Row <= otherRange.End.Row && otherRange.Start.Row <= thisRange.End.Row
                   );
        }

        public static ExcelRange Intersect(this ExcelRange thisRange, ExcelRange otherRange)
        {
            if (!thisRange.Intersecting(otherRange))
                return null;

            return thisRange.Worksheet.Cells[
                Math.Max(thisRange.Start.Row, otherRange.Start.Row),
                Math.Max(thisRange.Start.Column, otherRange.Start.Column),
                Math.Min(thisRange.End.Row, otherRange.End.Row),
                Math.Min(thisRange.End.Column, otherRange.End.Column)
                ];
        }

        public static ExcelRange Intersect(this ExcelRange thisRange, string otherRangeAddress)
        {
            return thisRange.Intersect(thisRange.Worksheet.Cells[otherRangeAddress]);
        }

        #endregion

        #region ExcelWorkbook

        //public static ExcelTable FillSheet(this ExcelWorkbook workbook)
        //{
        //    return null;
        //}

        #endregion

        #region ICategoryDistributionReport

        private static DateTime CurrentIssueDate(DateTime reportDate)
        {
            return reportDate.AddDays(1 - reportDate.Day);
        }

        private static DateTime NextIssueDate(DateTime reportDate)
        {
            return CurrentIssueDate(reportDate).AddMonths(1);
        }

        #endregion

        #region IReport

        internal static void ExecuteNonQuery(this IReport report, SqlConnection connection, string embeddedResourceName)
        {
            Common.ExecuteNonQuery(connection, report.GetReformattedParameters(), embeddedResourceName);
        }

        internal static DataTable ExecuteDataTable(this IReport report, SqlConnection connection, string embeddedResourceName)
        {
            return Common.ExecuteDataTable(connection, report.GetReformattedParameters(), embeddedResourceName);
        }

        #endregion

        #region Version

        internal static DateTime ToDateTime(this Version version)
        {

            DateTime t = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

            //if (TimeZone.IsDaylightSavingTime(DateTime.Now, TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year)))
            //    t = t.AddHours(1);

            return t;
        }

        #endregion

    }
}
