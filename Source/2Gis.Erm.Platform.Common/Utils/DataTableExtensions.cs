using System;
using System.Data;
using System.Text;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class DataTableExtensions
    {
        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] CharactersToEscape = { ';', ',', '"', '\n' };

        public static string ToCsv(this DataTable table)
        {
            return ToCsv(table, ",");
        }

        public static string ToCsv(this DataTable table, string separator)
        {
            return ToCsv(table, separator, false);
        }

        public static string ToCsv(this DataTable table, string separator, bool renderColumnNames)
        {
            // Семейство этих методов давно используется без экранирования. 
            // Поэтому по умолчанию он продолжает использоваться без экранирования, 
            // хотя это и не лучшая идея, но есть опасение поломать давно устояшийся порядок вещей.
            return ToCsv(table, separator, renderColumnNames, false);
        }

        public static string ToCsvEscaped(this DataTable table, string separator, bool renderColumnNames)
        {
            return ToCsv(table, separator, renderColumnNames, true);
        }

        public static string ToCsv(this DataTable table, string separator, bool renderColumnNames, bool escape)
        {
            var result = new StringBuilder();
            var escapeValue = escape
                                  ? EscapeCsvString
                                  : (Func<string, string>)(s => s);

            if (renderColumnNames)
            {
                for (var i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(table.Columns[i].ColumnName);
                    result.Append(separator);
                }

                result.Append(Environment.NewLine);
            }

            foreach (DataRow row in table.Rows)
            {
                for (var i = 0; i < table.Columns.Count - 1; i++)
                {
                    result.Append(escapeValue(row[i].ToString()));
                    result.Append(separator);
                }

                // last one
                result.Append(escapeValue(row[table.Columns.Count - 1].ToString()));
                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        private static string EscapeCsvString(string value)
        {
            if (value.Contains(Quote))
            {
                value = value.Replace(Quote, EscapedQuote);
            }

            if (value.IndexOfAny(CharactersToEscape) > -1)
            {
                value = Quote + value + Quote;
            }

            return value;
        }
    }
}