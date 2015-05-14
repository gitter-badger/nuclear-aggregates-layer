using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;

using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace DoubleGis.Erm.BL.Reports.PlanningReport
{
    public sealed class PlanningReport : SpreadsheetReportBase
    {

        #region Constructors

        public PlanningReport()
            : base(
                new Dictionary<string, object>
                    {
                        { "@City", Common.CITY },
                        { "@IssueDate", Common.ISSUEDATE },
                        { "@IsAdvertisingAgency", false }
                    },
                new Dictionary<string, object>
                    {
                        { "Connection", Common.ERM_CONNECTION }
                    })
        {
        }

        public PlanningReport(SqlConnection connection, long city, DateTime issueDate, bool isAdvertisingAgency)
            : base(
                new Dictionary<string, object>
                    {
                        { "@City", city },
                        { "@IssueDate", issueDate },
                        { "@IsAdvertisingAgency", isAdvertisingAgency },
                    },
                new Dictionary<string, object>
                    {
                        { "Connection", connection }
                    })
        {

        }

        #endregion

        public override string ReportName
        {
            get { return "Отчет по планированию"; }
        }

        private void MakeCoefficientCell(ExcelRange range, object defaultValue)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.Red);
            range.Style.Font.Bold = true;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Numberformat.Format = "#,##0.00";
            range.Value = defaultValue;
        }

        private void StyleGrayBckGrnd(ExcelRange range)
        {

            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
            StyleBoldNumber(range);
        }

        private void StyleGreenBckGrnd(ExcelRange range)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(215, 228, 188));
            StyleBoldNumber(range);
        }

        private void StyleBoldNumber(ExcelRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Numberformat.Format = "#,##0";
        }

        private void StyleBorder(ExcelRange range)
        {
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        private void FillDelimeterRow(ExcelWorksheet worksheet, int rowIndex)
        {
            var currentRowRange = worksheet.Cells[rowIndex, 1, rowIndex, worksheet.Dimension.End.Column];

            currentRowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            currentRowRange.Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            MakeCoefficientCell(worksheet.Cells[rowIndex, 12], 1.00);
            MakeCoefficientCell(worksheet.Cells[rowIndex, 13], 1.00);

            if (rowIndex == 2)
            {
                MakeCoefficientCell(worksheet.Cells[rowIndex, 37], 2.50);
                MakeCoefficientCell(worksheet.Cells[rowIndex, 38], 1.00);
                MakeCoefficientCell(worksheet.Cells[rowIndex, 40], 1.00);
                MakeCoefficientCell(worksheet.Cells[rowIndex, 44], 1.00);
            }
        }

        private class Group
        {
            public Group(string groupMaster, int groupOrdinal, int countMembers)
            {
                GroupMaster = groupMaster;
                GroupOrdinal = groupOrdinal;
                CountMembers = countMembers;
            }

            public string GroupMaster { get; set; }
            public int GroupOrdinal { get; set; }
            public int CountMembers { get; set; }
        }

        private void FillSheet_Служебная(ExcelWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Служебная");
            sheet.DefaultColWidth = 13.29;
            sheet.View.FreezePanes(2, 3);

            int i = 0;
            sheet.Cells[1, ++i].Value = "№ п/п";
            sheet.Column(i).Width = 4.71;
            sheet.Cells[1, ++i].Value = "ФИО";
            sheet.Column(i).Width = 31.86;
            sheet.Cells[1, ++i].Value = "Квалификация";
            sheet.Cells[1, ++i].Value = "Плановое количество рабочих дней";
            sheet.Cells[1, ++i].Value = "Текущее размещение (гарантированный объем)";
            sheet.Cells[1, ++i].Value = "Текущее размещение (гарантированный объем) по прайс-листу";
            sheet.Cells[1, ++i].Value = "Гарантированные отгрузки (объем в выпуск, справочно)";
            sheet.Cells[1, ++i].Value = "К продлению в выпуск (справочно из CRM)";
            sheet.Cells[1, ++i].Value = "К продлению в выпуск (справочно из CRM) по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по продлениям в выпуск";
            sheet.Cells[1, ++i].Value = "План по продлениям в выпуск по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по продлениям в выпуск с коэффициентом";
            sheet.Cells[1, ++i].Value = "План по продлениям в выпуск с коэффициентом по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по оплаченному объему продлений в выпуск";
            sheet.Cells[1, ++i].Value = "Кол-во работ к продлению";
            sheet.Cells[1, ++i].Value = "Кол-во новых продаж в шт";
            sheet.Cells[1, ++i].Value = "Средний чек по новым продажам";
            sheet.Cells[1, ++i].Value = "Средний чек по новым продажам по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по новым продажам в выпуск";
            sheet.Cells[1, ++i].Value = "План по новым продажам в выпуск по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по оплаченному объему по новым в выпуск";
            sheet.Cells[1, ++i].Value = "Кол-во прочих продаж в шт";
            sheet.Cells[1, ++i].Value = "Средний чек по прочим продажам";
            sheet.Cells[1, ++i].Value = "Средний чек по прочим продажам по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по прочим продажам в выпуск";
            sheet.Cells[1, ++i].Value = "План по прочим продажам в выпуск по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по оплаченному объему по прочим в выпуск";
            sheet.Cells[1, ++i].Value = "Объем рекламы в выпуск от продления и увеличения";
            sheet.Cells[1, ++i].Value = "Объем рекламы в выпуск от продления и увеличения по прайс-листу";
            sheet.Cells[1, ++i].Value = "План по оплаченному объему рекламы в выпуск от продления и увеличения";
            sheet.Cells[1, ++i].Value = "Итого объем рекламы в выпуск";
            sheet.Cells[1, ++i].Value = "Итого объем рекламы в выпуск по прайс-листу";
            sheet.Cells[1, ++i].Value = "Прогноз";
            sheet.Cells[1, ++i].Value = "Прогноз по прайс-листу";
            sheet.Cells[1, ++i].Value = "Увеличение объема рекламы в выпуск от прогноза";
            sheet.Cells[1, ++i].Value = "Увеличение объема рекламы в выпуск от прогноза по прайс-листу";
            sheet.Cells[1, ++i].Value = "Предоплата по новым";
            sheet.Cells[1, ++i].Value = "Предоплата по прочим";
            sheet.Cells[1, ++i].Value = "Предоплата по продлениям ";
            sheet.Cells[1, ++i].Value = "Предоплата по продлениям с коэффициентом";
            sheet.Cells[1, ++i].Value = "Оплаты по ДЗ накопленной до 01.12.2014";
            sheet.Cells[1, ++i].Value = "Поступления по рассрочкам  (кроме оплат ДЗ до 01.12.2014)";
            sheet.Cells[1, ++i].Value = "Поступления по дебитоpке (кроме оплат ДЗ до 01.12.2014)";
            sheet.Cells[1, ++i].Value = "Поступления по дебиторке с коэффициентом  (кроме оплат ДЗ до 01.12.2014)";
            sheet.Cells[1, ++i].Value = "План по оплатам";
            sheet.Cells[1, ++i].Value = "Полная дебиторская задолженность филиала (Продажи) Справочно";
            sheet.Cells[1, ++i].Value = "Дебиторская задолженность филиала, накопленная с 01.12.2014 (Продажи) Справочно";
            sheet.Cells[1, ++i].Value = "Обоснование плана по продлениям";
            sheet.Cells[1, ++i].Value = "Обоснование плана по новым продажам";
            sheet.Cells[1, ++i].Value = "Обоснование плана по оплатам";

            var headerRange = sheet.Cells[1, 1, 1, i];

            headerRange.Worksheet.Row(headerRange.Start.Row).Height = 78.75;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));
            headerRange.Style.WrapText = true;
            headerRange.Style.Font.Size = 8;
            sheet.Cells[1, 30].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(215, 228, 188));
            sheet.Cells[1, 45].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(215, 228, 188));

            var groups = new List<Group>();

            var dataTable = ExecuteDataTable(@"PlanningReport.Лист_Служебная.sql");
            Group currentGroup = null;
            int currentRow = 1;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                currentRow++;

                var groupOrdinal = (int)dataRow["GroupOrdinal"];
                var ordinal = (int)dataRow["Ordinal"];
                var displayName = (string)dataRow["DisplayName"];
                var isGm = (bool)dataRow["IsGM"];
                var gM = (string)dataRow["GM"];

                if (currentGroup == null || currentGroup.GroupOrdinal != groupOrdinal)
                {
                    currentGroup = new Group(gM, groupOrdinal, 1);
                    groups.Add(currentGroup);

                    FillDelimeterRow(sheet, currentRow);

                    currentRow++;
                }
                else
                {
                    currentGroup.CountMembers++;
                }

                int currentColumn = 0;


                sheet.Cells[currentRow, currentColumn = 1].Value = ordinal;

                sheet.Cells[currentRow, currentColumn = 2].Value = displayName;

                sheet.Cells[currentRow, currentColumn = 3].Value = isGm ? "Рук. Группы" : null;
                sheet.Cells[currentRow, currentColumn].Style.Font.Bold = true;

                sheet.Cells[currentRow, currentColumn = 4].Value = 20;
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 5].Formula = String.Format("SUMIF(Текущие!B:B,B{0},Текущие!K:K)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 6].Formula = String.Format("SUMIF(Текущие!B:B,B{0},Текущие!L:L)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 7].Formula = String.Format("SUMIF('Гарантированные отгрузки'!A:A,B{0},'Гарантированные отгрузки'!H:H)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 8].Formula = String.Format("SUMIF('Продление'!A:A,B{0},'Продление'!L:L)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 9].Formula = String.Format("SUMIF('Продление'!A:A,B{0},'Продление'!M:M)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 10].Formula = String.Format("SUMIF('Продление'!A:A,B{0},'Продление'!P:P)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 11].Formula = String.Format("SUMIF('Продление'!A:A,B{0},'Продление'!Q:Q)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 12].Formula = String.Format("J{0}*$L${1}", currentRow, currentRow - ordinal);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 13].Formula = String.Format("K{0}*$M${1}", currentRow, currentRow - ordinal);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
             
                sheet.Cells[currentRow, currentColumn = 14].Formula = String.Format("SUMIF('Продление'!A:A,B{0},'Продление'!V:V)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 15].Formula = String.Format("COUNTIF('Продление'!A:A,B{0})", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 19].Formula = String.Format("P{0}*Q{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 20].Formula = String.Format("P{0}*R{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 21].Formula = String.Format("IF(S{0}=0,0,IF(AK{0}<=S{0},AK{0},S{0}))", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 25].Formula = String.Format("V{0}*W{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 26].Formula = String.Format("V{0}*X{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 27].Formula = String.Format("IF(Y{0}=0,0,IF(AL{0}<=Y{0},AL{0},Y{0}))", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 28].Formula = String.Format("L{0}+S{0}+Y{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 29].Formula = String.Format("M{0}+T{0}+Z{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 30].Formula = String.Format("N{0}+U{0}+AA{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGreenBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 31].Formula = String.Format("E{0}+AB{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 32].Formula = String.Format("F{0}+AC{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 33].Formula = String.Format("E{0}+H{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 34].Formula = String.Format("F{0}+I{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 35].Formula = String.Format("AE{0}-AG{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 36].Formula = String.Format("AF{0}-AH{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 37].Formula = String.Format("S{0}*$AK$2", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 38].Formula = String.Format("Y{0}*$AL$2", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 39].Formula = String.Format("SUMIF('Продление'!A:A,B{0},'Продление'!S:S)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 40].Formula = String.Format("AM{0}*$AN$2", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 41].Formula = String.Format("SUMIF('Оплаты по ДЗ до 01.12.2014'!A:A,B{0},'Оплаты по ДЗ до 01.12.2014'!F:F)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 42].Formula = String.Format("SUMIF('Рассрочки'!A:A,B{0},'Рассрочки'!H:H)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 43].Formula = String.Format("SUMIF('Поступления по ДЗ'!A:A,B{0},'Поступления по ДЗ'!E:E)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 44].Formula = String.Format("AQ{0}*$AR$2", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 45].Formula = String.Format("AK{0}+AN{0}+AP{0}+AR{0}+AL{0}+AO{0}", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGreenBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 46].Formula = String.Format("SUMIF('Полная ДЗ (справочно)'!A:A,B{0},'Полная ДЗ (справочно)'!C:C)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 47].Formula = String.Format("SUMIF('ДЗ накопленная с 01.12.2014'!C:C,B{0},'ДЗ накопленная с 01.12.2014'!D:D)", currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);
            }

            if (sheet.Dimension != null)
            {
                var usedRange = sheet.Cells[sheet.Dimension.Address];
                StyleBorder(usedRange);
            }

            currentRow += 2;

            sheet.Cells[currentRow, 2].Value = "Группы";
            sheet.Cells[currentRow, 2].Style.Font.Bold = true;
            StyleGrayBckGrnd(sheet.Cells[currentRow, 2]);
            StyleBorder(sheet.Cells[currentRow, 2]);

            int lastMemberRow = 2;

            foreach (var group in groups)
            {
                currentRow++;

                int currentColumn = 0;


                sheet.Cells[currentRow, currentColumn = 1].Value = group.GroupOrdinal;

                sheet.Cells[currentRow, currentColumn = 2].Value = group.GroupMaster;

                sheet.Cells[currentRow, currentColumn = 3].Value = "Рук. Группы";
                sheet.Cells[currentRow, currentColumn].Style.Font.Bold = true;

                sheet.Cells[currentRow, currentColumn = 4, currentRow, headerRange.End.Column].FormulaR1C1 =
                    String.Format("SUM(R[{0}]C:R[{1}]C)", lastMemberRow + 1 - currentRow,
                                  lastMemberRow + group.CountMembers - currentRow);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn, currentRow, headerRange.End.Column]);
                StyleBorder(sheet.Cells[currentRow, 1, currentRow, headerRange.End.Column]);

                StyleGrayBckGrnd(sheet.Cells[currentRow, 5]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 6]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 8]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 9]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 19]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 20]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 28]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 29]);
                StyleGreenBckGrnd(sheet.Cells[currentRow, 30]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 31]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 32]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 41]);
                StyleGreenBckGrnd(sheet.Cells[currentRow, 45]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 46]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 47]);

                lastMemberRow += group.CountMembers + 1;
            }

            currentRow += 3;

            {
                int currentColumn = 0;

                sheet.Cells[currentRow, currentColumn = 2].Value = "Итого";
                sheet.Cells[currentRow, currentColumn].Style.Font.Bold = true;
                StyleGrayBckGrnd(sheet.Cells[currentRow, currentColumn]);

                sheet.Cells[currentRow, currentColumn = 4, currentRow, headerRange.End.Column].FormulaR1C1 =
                    String.Format("SUM(R[-{0}]C:R[-{1}]C)", groups.Count + 2, 3);
                StyleBoldNumber(sheet.Cells[currentRow, currentColumn, currentRow, headerRange.End.Column]);
                StyleBorder(sheet.Cells[currentRow, 1, currentRow, headerRange.End.Column]);

                StyleGrayBckGrnd(sheet.Cells[currentRow, 5]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 6]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 8]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 9]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 19]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 20]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 28]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 29]);
                StyleGreenBckGrnd(sheet.Cells[currentRow, 30]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 31]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 32]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 41]);
                StyleGreenBckGrnd(sheet.Cells[currentRow, 45]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 46]);
                StyleGrayBckGrnd(sheet.Cells[currentRow, 47]);
            }

            sheet.Cells[sheet.Dimension.Address].Style.Font.Name = "Arial";
            sheet.Cells[sheet.Dimension.Address].Style.Font.Size = 8;
            sheet.View.ZoomScale = 80;

            //sheet.Protection.SetPassword("123654");
        }

        private string ReportWideName
        {
            get
            {
                return String.Format("Отчет по планам филиала на {0}",
                              ((DateTime)Parameters["@IssueDate"]).ToString("MMMM yyyy", new CultureInfo("ru-RU")));
            }
        }

        private void FillSheet_Сводные_планы(ExcelWorkbook workbook)
        {
            var sheet = workbook.Worksheets.Add("Сводные планы");

            sheet.Column(1).Width = 77.86;
            sheet.Column(2).Width = 47.71;

            sheet.Cells[1, 1].Value = ReportWideName;

            var mergedCaption = sheet.Cells[1, 1, 1, 2];
            mergedCaption.Merge = true;

            mergedCaption.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            mergedCaption.Style.Font.Bold = true;

            sheet.Cells[3, 1, 3, 2].Style.Font.Bold = true;
            StyleBoldNumber(sheet.Cells[3, 2, 12, 2]);
            StyleBorder(sheet.Cells[3, 1, 12, 2]);

            sheet.Cells[3, 1].Value = "Показатель";
            sheet.Cells[3, 2].Value = "Плановое значение показателя по филиалу";

            sheet.Cells[4, 1].Value = "Текущее размещение (гарантированный объем)";
            sheet.Cells[5, 1].Value = "К продлению в выпуск (справочно из CRM)";
            sheet.Cells[6, 1].Value = "План по новым продажам в выпуск";
            sheet.Cells[7, 1].Value = "Объем рекламы в выпуск от продления и увеличения (прирост)";
            sheet.Cells[8, 1].Value = "План по оплаченному объему рекламы в выпуск от продления и увеличения";
            sheet.Cells[9, 1].Value = "Итого объем рекламы в выпуск";
            sheet.Cells[10, 1].Value = "Оплаты по ДЗ накопленной до 01.12.2014";
            sheet.Cells[11, 1].Value = "План по оплатам";
            sheet.Cells[12, 1].Value = "Полная дебиторская задолженность филиала (Продажи) Справочно";

            var totalsRow = workbook.Worksheets["Служебная"].Dimension.End.Row;

            sheet.Cells[4, 2].Formula = String.Format("Служебная!E{0}", totalsRow);
            sheet.Cells[5, 2].Formula = String.Format("Служебная!H{0}", totalsRow);
            sheet.Cells[6, 2].Formula = String.Format("Служебная!S{0}", totalsRow);
            sheet.Cells[7, 2].Formula = String.Format("Служебная!AB{0}", totalsRow);
            sheet.Cells[8, 2].Formula = String.Format("Служебная!AD{0}", totalsRow);
            sheet.Cells[9, 2].Formula = String.Format("Служебная!AE{0}", totalsRow);
            sheet.Cells[10, 2].Formula = String.Format("Служебная!AO{0}", totalsRow);
            sheet.Cells[11, 2].Formula = String.Format("Служебная!AS{0}", totalsRow);
            sheet.Cells[12, 2].Formula = String.Format("Служебная!AT{0}", totalsRow);

            var usedRange = sheet.Cells[sheet.Dimension.Address];
            usedRange.Style.Font.Name = "Verdana";
            usedRange.Style.Font.Size = 10;
        }

        public override string DefaultFileName
        {
            get { return String.Format("{0}.xlsx", ReportWideName); }
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

            // важно, что параметров к запросу нет и он выполняется не внутри sp_executesql
            Common.ExecuteNonQuery(connection, new ProtectedDictionary(), @"PlanningReport.CreateTemporaryTable.sql");

            this.ExecuteNonQuery(connection, @"PlanningReport.Расчет_групп_МПП.sql");

            table = FillSheetFromMssql(package.Workbook, "Текущие", @"PlanningReport.Лист_Текущие.sql");
            table = FillSheetFromMssql(package.Workbook, "Гарантированные отгрузки", @"PlanningReport.Лист_Гарантированные_отгрузки.sql");

            table = FillSheetFromMssql(package.Workbook, "Продление", @"PlanningReport.Лист_Продление.sql");
            usedRange = table.GetDataRange();

            usedRange.Intersect("P:P").Style.Numberformat.Format = "#,##0.00\"р.\"";
            usedRange.Intersect("Q:Q").Style.Numberformat.Format = "#,##0.00\"р.\"";
            usedRange.Intersect("S:S").Style.Numberformat.Format = "#,##0.00\"р.\"";
            usedRange.Intersect("T:T").FormulaR1C1 = "IF(RC[-4]=0,0,RC[-1]/RC[-4])";
            usedRange.Intersect("U:U").FormulaR1C1 = "IF(RC[-1]=0,0,RC[-1]-RC[-6])";
            usedRange.Intersect("V:V").FormulaR1C1 = "IF(RC[-6]=0,0,IF(RC[-3]<=RC[-6],RC[-3],RC[-6]))";

            table = FillSheetFromMssql(package.Workbook, "Оплаты по ДЗ до 01.12.2014", @"PlanningReport.Лист_Оплаты_по_ДЗ_до_01_12.sql");
            table.WorkSheet.Column(3).Hidden = true;
            table.WorkSheet.Column(4).Hidden = true;

            table = FillSheetFromMssql(package.Workbook, "Рассрочки", @"PlanningReport.Лист_Рассрочки.sql");
            usedRange = table.GetDataRange();

            //Типа условное форматирование
            for (int i = usedRange.Start.Row; i <= usedRange.End.Row; i++)
            {
                var rowRange = table.WorkSheet.Cells[i, usedRange.Start.Column, i, usedRange.End.Column];

                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;

                var value1 = (decimal?)table.WorkSheet.Cells[i, 5].Value;
                var value2 = (decimal?)table.WorkSheet.Cells[i, 6].Value;

                if (value1.HasValue && value2.HasValue)
                {
                    var color = value1 < value2
                                ? Color.FromArgb(239, 175, 140)
                                : value1 > value2 ? Color.FromArgb(127, 199, 255) : Color.White;
                    rowRange.Style.Fill.BackgroundColor.SetColor(color);
                }
            }

            table = FillSheetFromMssql(package.Workbook, "Поступления по ДЗ", @"PlanningReport.Лист_Поступления_по_ДЗ.sql");
            table = FillSheetFromMssql(package.Workbook, "Полная ДЗ (справочно)", @"PlanningReport.Лист_Полная_ДЗ.sql");

            table = FillSheetFromMssql(package.Workbook, "ДЗ накопленная с 01.12.2014", @"PlanningReport.Лист_ДЗ_накопленная_с_01_12.sql");
            table.WorkSheet.Column(2).Hidden = true;
            table.WorkSheet.Column(5).Hidden = true;
            table.WorkSheet.Column(6).Hidden = true;
            table.WorkSheet.Column(7).Hidden = true;

            FillSheet_Служебная(package.Workbook);

            FillSheet_Сводные_планы(package.Workbook);

            if (!connectionWasOpened)
                connection.Close();
            return package;
        }
    }
}
