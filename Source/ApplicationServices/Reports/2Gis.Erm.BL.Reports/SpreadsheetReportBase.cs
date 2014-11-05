using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Transactions;

using OfficeOpenXml;
using OfficeOpenXml.Table;

using IsolationLevel = System.Transactions.IsolationLevel;

namespace DoubleGis.Erm.BL.Reports
{
    public abstract class SpreadsheetReportBase : ReportBase
    {
        protected SpreadsheetReportBase(Dictionary<string, object> parameters, Dictionary<string, object> connections)
            : base(parameters, connections)
        {
        }

        #region protected members

        /// <summary>
        /// Метод реализует всю логику отчета, в нем должны произвестись все необходимые действия для заполнения отчета данными и их форматирования
        /// </summary>
        /// <returns></returns>
        protected override abstract ExcelPackage Execute();

        protected ExcelTable InsertDataTableToSheet(ExcelWorksheet worksheet, DataTable dataTable, bool withFormatting)
        {
            return InsertDataTableToSheet(worksheet, dataTable, "A1", true, withFormatting);
        }

        protected ExcelTable InsertDataTableToSheet(ExcelWorksheet worksheet, DataTable dataTable)
        {
            return InsertDataTableToSheet(worksheet, dataTable, "A1", true, false);
        }

        protected ExcelTable InsertDataTableToSheet(ExcelWorksheet worksheet, DataTable dataTable, string startingCell, bool withHeader, bool withFormatting)
        {
            if (dataTable == null)
                return null;

            ExcelCellAddress startingCellAddress = new ExcelCellAddress(startingCell);

            worksheet.Cells[startingCellAddress.Address].LoadFromDataTable(dataTable, withHeader);

            ExcelTable table =
                worksheet.Tables.Add(
                    new ExcelAddressBase(startingCellAddress.Row, startingCellAddress.Column,
                                         (dataTable.Rows.Count == 0 ? 1 : dataTable.Rows.Count) + startingCellAddress.Row - 1 +
                                         (withHeader ? 1 : 0),
                                         dataTable.Columns.Count + startingCellAddress.Column - 1), dataTable.TableName);

            table.ShowHeader = withHeader;
            table.TableStyle = TableStyles.Light9;
            table.ShowColumnStripes = true;
            table.ShowRowStripes = true;



            #region Форматирование

            //Форматирование ОЧЕНЬ условное,
            //поэтому если что-то не так, отключаем его и все форматирование делаем вручную в методе Execute

            if ((!withFormatting) || dataTable.Rows.Count == 0)
                return table;

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                ExcelRangeBase tableColumnRange = worksheet.Cells[table.Address.Address].Offset((withHeader ? 1 : 0), dataColumn.Ordinal, dataTable.Rows.Count, 1);
                var sheetColumn = worksheet.Column(dataColumn.Ordinal + startingCellAddress.Column);

                if (withFormatting)
                {
                    switch (dataColumn.DataType.FullName)
                    {
                        case "System.DateTime":
                            sheetColumn.AutoFit2(17.0);
                            tableColumnRange.Style.Numberformat.Format = "dd.MM.yyyy";

                            break;
                        case "System.Single":
                        case "System.Decimal":
                            //Как отличить деньги от процентов?
                            //Просматриваем всю колонку, и если все значения в пределах от -1 до 1, значит проценты
                            //иначе деньги
                            sheetColumn.AutoFit2(17.0);
                            //Если все значения нули, то... на "нет" и суда нет
                            decimal dummy = 0;
                            if (tableColumnRange.All(item => !Decimal.TryParse(item.Value.ToString(), out dummy) || (dummy == 0)))
                                break;

                            tableColumnRange.Style.Numberformat.Format =
                                tableColumnRange.Where(
                                    item =>
                                    item.Start.Row >= startingCellAddress.Row + (withHeader ? 1 : 0) && item.Value != null &&
                                    item.Value.ToString() != ""
                                    ).All(
                                        item => Math.Abs((decimal)item.Value) <= 1
                                    )
                                    ? "0.00%"
                                    : "#,##0.00\"р.\"";
                            break;
                        case "System.String":
                            sheetColumn.AutoFit2(35.0);
                            break;
                    }
                }
            }

            #endregion

            return table;
        }

        protected ExcelTable FillSheetFromMssql(ExcelWorkbook workbook, string sheetName, string resourceName)
        {
            return FillSheetFromMssql(workbook, sheetName, resourceName, true, true);
        }


        protected ExcelTable FillSheetFromMssql(ExcelWorkbook workbook, string sheetName, string resourceName, bool withHeader, bool withFormatting)
        {
            var workSheet = workbook.Worksheets[sheetName] ?? workbook.Worksheets.Add(sheetName);

            var dataTable = ExecuteDataTable(resourceName);

            return InsertDataTableToSheet(workSheet, dataTable, "A1", withHeader, withFormatting);
        }

        public void AddPackageInfo(ExcelPackage package)
        {
            var properties = package.Workbook.Properties;
            properties.SetCustomPropertyValue("MachineName", Environment.MachineName);
            properties.SetCustomPropertyValue("ReportName", ReportName);
            properties.SetCustomPropertyValue("ReportClass", GetType().FullName);
            properties.SetCustomPropertyValue("AssemblyVersion", GetType().Assembly.GetName().Version.ToString());
            properties.SetCustomPropertyValue("AssemblyBuildDate", GetType().Assembly.GetName().Version.ToDateTime().ToString("dd.MM.yyyy HH:mm:ss"));
            properties.SetCustomPropertyValue("Parameters", Parameters.ToString());
        }


        #endregion

        private ExcelPackage ExecutePackage()
        {
            ExcelPackage package;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {IsolationLevel = IsolationLevel.Serializable}))
            {
                package = Execute();
                AddPackageInfo(package);
                transaction.Complete();
            }

            return package;
        }

        public override void SaveAs(string filePath)
        {
            string path = filePath;
            if (Path.GetExtension(filePath) != ".xlsx")
                path += ".xlsx";

            var package = ExecutePackage();


            if (File.Exists(path))
                File.Delete(path);

            package.SaveAs(new FileInfo(path));
        }

        public override Stream ExecuteStream()
        {
            return new MemoryStream(ExecutePackage().GetAsByteArray());
        }

        public virtual string DefaultFileName
        {
            get { return String.Format("{0}.xlsx", ReportName); }
        }
    }
}