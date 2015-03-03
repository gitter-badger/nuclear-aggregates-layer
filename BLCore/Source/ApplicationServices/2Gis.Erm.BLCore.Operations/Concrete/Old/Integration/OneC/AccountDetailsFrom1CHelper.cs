using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    public static class AccountDetailsFrom1CHelper
    {
        internal enum InternalOperationType
        {
            // списание
            Withdrawal = 0,

            // начисление
            Charge = 1
        }

        public static string[] ParseStreamAsRows(Stream inputStream, Encoding useEncoding)
        {
            string content;
            using (var streamReader = new StreamReader(inputStream, useEncoding))
            {
                content = streamReader.ReadToEnd();
            }

            var rows = new List<string>();
            using (var stringReader = new StringReader(content))
            {
                for (var row = stringReader.ReadLine(); row != null; row = stringReader.ReadLine())
                {
                    if (!string.IsNullOrWhiteSpace(row))
                    {
                        rows.Add(row);
                    }
                }
            }

            return rows.ToArray();
        }

        private static string TrimAndReplaceEmptyString(string input)
        {
            var result = input.Trim();
            return result.Length > 0 ? result : null;
        }

        internal sealed class CsvHeader
        {
            public readonly TimePeriod Period;
            public readonly string BranchOfficeOrganizationUnit1CCode;

            private CsvHeader(TimePeriod period, string branchOfficeOrganizationUnit1CCode)
            {
                Period = period;
                BranchOfficeOrganizationUnit1CCode = branchOfficeOrganizationUnit1CCode;
            }

            public static bool TryParse(string headerRow, CultureInfo useCulture, out CsvHeader csvHeader)
            {
                csvHeader = null;

                var headerInfo = headerRow.Split(';');
                if (!headerInfo.Any())
                {
                    throw new NotificationException("Формат сообщения не соответствует ожидаемому формату, проверьте сообщение");
                }

                if (headerInfo.Length > 2)
                {
                    throw new NotificationException("Формат сообщения не соответствует ожидаемому формату, проверьте сообщение");
                }

                // parse import period
                var periodNonParsed = headerInfo[0].Split(' ');
                if (periodNonParsed.Length < 2)
                {
                    throw new NotificationException("Формат сообщения не соответствует ожидаемому формату, проверьте сообщение");
                }

                DateTime start;
                if (!DateTime.TryParse(periodNonParsed[0], useCulture, DateTimeStyles.None, out start))
                {
                    return false;
                }

                DateTime end;
                if (!DateTime.TryParse(periodNonParsed[1], useCulture, DateTimeStyles.None, out end))
                {
                    return false;
                }

                csvHeader = new CsvHeader(new TimePeriod(start, end), headerInfo[1].Trim());
                return true;
            }
        }

        internal sealed class CsvRow
        {
            public string BranchOfficeOrganizationUnit1CCode { get; private set; }
            public string LegalPerson1CCode { get; private set; }
            public string InnOrPassportSeries { get; private set; }
            public string KppOrPassportNumber { get; private set; }
            public decimal Amount { get; private set; }
            public DateTime OperationDate { get; private set; }
            public InternalOperationType OperationType { get; private set; }
            public string DocumentType { get; private set; }
            public string DocumentNumber { get; private set; }
            public DateTime DocumentDate { get; private set; }

            public static bool TryParse(string row, CultureInfo useCulture, out CsvRow csvRow)
            {
                csvRow = null;
                var rowInfo = row.Split(';');

                if (rowInfo.Length < 9)
                {
                    return false;
                }

                // parse amount in CsvCulture or in InvariantCulture
                decimal amount;
                if (!decimal.TryParse(rowInfo[4], NumberStyles.Float, useCulture, out amount) &&
                    !decimal.TryParse(rowInfo[4], NumberStyles.Float, CultureInfo.InvariantCulture, out amount))
                {
                    return false;
                }

                // parse operation date
                DateTime operationDate;
                if (!DateTime.TryParse(rowInfo[5], useCulture, DateTimeStyles.None, out operationDate))
                {
                    return false;
                }

                // parse operation type
                int operationType;
                if (!int.TryParse(rowInfo[6], NumberStyles.Number, useCulture, out operationType))
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(InternalOperationType), operationType))
                {
                    return false;
                }

                // parse document date
                DateTime documentDate;
                if (!DateTime.TryParse(rowInfo[9], useCulture, DateTimeStyles.None, out documentDate))
                {
                    return false;
                }

                csvRow = new CsvRow
                {
                    BranchOfficeOrganizationUnit1CCode = rowInfo[0].Trim(),
                    LegalPerson1CCode = rowInfo[1].Trim(),
                    InnOrPassportSeries = TrimAndReplaceEmptyString(rowInfo[2]),
                    KppOrPassportNumber = TrimAndReplaceEmptyString(rowInfo[3]),
                    Amount = amount,
                    OperationDate = operationDate,
                    OperationType = (InternalOperationType)operationType,
                    DocumentType = rowInfo[7].Trim(),
                    DocumentNumber = rowInfo[8].Trim(),
                    DocumentDate = documentDate,
                };

                return true;
            }
        }
    }
}
