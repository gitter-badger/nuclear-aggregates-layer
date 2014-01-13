using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace DoubleGis.Erm.BLCore.Reports.LegalPersonPayments
{
    public sealed class LegalPersonPayments : SpreadsheetReportBase
    {
        #region Constructors

        public LegalPersonPayments()
            : base(
                new Dictionary<string, object>
                {
                    { "@City", Common.CITY },
                    { "@CurrentUser", Common.CURRENT_USER },
                    { "@ReportDate", Common.ISSUEDATE },
                    { "@UseOnRegistration", false },
                    { "@UseOnApproval", false }
                },
                new Dictionary<string, object>
                {
                    { "Connection", Common.ERM_CONNECTION }
                })
        {
        }

        public LegalPersonPayments(
            Dictionary<string, object> parameters,
            Dictionary<string, object> connections)
            : base(parameters, connections)
        {
        }

        //устаревший конструктор, оставлен для обратной совместимости
        public LegalPersonPayments(SqlConnection connection, long city, DateTime issueDate, long currentUser)
            : base(
                new Dictionary<string, object>()
                    {
                        { "@City", city },
                        { "@CurrentUser", currentUser },
                        { "@ReportDate", issueDate },
                        { "@UseOnRegistration", false },
                        { "@UseOnApproval", false }
                    },
                new Dictionary<string, object>()
                    {
                        { "Connection", connection }
                    })
        {
        }

        public LegalPersonPayments(SqlConnection connection, long city, DateTime issueDate, long currentUser, bool useOnRegistration, bool useOnApproval)
            : base(
                new Dictionary<string, object>()
                    {
                        { "@City", city },
                        { "@CurrentUser", currentUser },
                        { "@ReportDate", issueDate },
                        { "@UseOnRegistration", useOnRegistration },
                        { "@UseOnApproval", useOnApproval }
                    },
                new Dictionary<string, object>()
                    {
                        { "Connection", connection }
                    })
        {
        }

        #endregion

        public override string ReportName
        {
            get { return "Отчет по оплатам юридических лиц"; }
        }

        private ExcelRange Expand(ExcelRange range)
        {
            return
                range.Worksheet.Cells[
                    range.Offset(
                        0,
                        0,
                        range.End.Row - range.Start.Row + 2,
                        range.End.Column - range.Start.Column + 1
                        ).Address
                    ];
        }

        protected override ExcelPackage Execute()
        {
            var package = new ExcelPackage();

            var connection = (SqlConnection)Connections["Connection"];

            var connectionWasOpened = connection.State == ConnectionState.Open;

            if (!connectionWasOpened)
                connection.Open();

            ExcelTable table;
            ExcelRange usedRange;

            this.ExecuteNonQuery(connection, @"LegalPersonPayments.Категории_и_распределение.sql");

            table = FillSheetFromMssql(package.Workbook, "Распределение оплат", @"LegalPersonPayments.Лист_Распреление_оплат.sql");
            table.WorkSheet.View.FreezePanes(2, 1);
            table.GetDataRange().Style.Numberformat.Format = "#,##0.00\"р.\"";
            table.EnableTotals();
            table.Columns[0].TotalsRowFormula = "\"Итого\"";
            table.Columns[1].TotalsRowFunction = RowFunctions.None;

            table = FillSheetFromMssql(package.Workbook, "Оплаты прироста", @"LegalPersonPayments.Лист_Оплаты_прироста.sql");
            table.WorkSheet.View.FreezePanes(2, 1);
            table.ShowTotal = true;
            table.Columns[0].TotalsRowFormula = "\"Итого\"";
            table.Columns[16].TotalsRowFunction = RowFunctions.Sum;
            table.Columns[17].TotalsRowFunction = RowFunctions.Sum;
            usedRange = Expand(table.GetDataRange());
            usedRange.Intersect("H:R").Style.Numberformat.Format = "#,##0.00\"р.\"";
            usedRange.Intersect("J:J").Style.Numberformat.Format = "";
            usedRange.Intersect("K:L").Style.Numberformat.Format = "dd.MM.yyyy";


            table = FillSheetFromMssql(package.Workbook, "Результаты по МПП", @"LegalPersonPayments.Лист_Результаты_по_МПП.sql");
            table.WorkSheet.View.FreezePanes(2, 1);
            table.GetDataRange().Style.Numberformat.Format = "#,##0.00\"р.\"";
            table.EnableTotals();
            table.Columns[0].TotalsRowFormula = "\"Итого\"";

            if (!connectionWasOpened)
                connection.Close();

            return package;
        }
    }
}