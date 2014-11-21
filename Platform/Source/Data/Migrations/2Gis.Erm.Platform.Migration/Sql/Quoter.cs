﻿using System;
using System.Globalization;

namespace DoubleGis.Erm.Platform.Migration.Sql
{
    public class GenericQuoter
    {
        private const string CloseQuoteEscapeString = "]]";
        private const string OpenQuote = "[";
        private const string CloseQuote = "]";

        public string OpenQuoteEscapeString
        {
            get { return string.Empty; }
        }

        public virtual string ValueQuote
        {
            get { return "'"; }
        }

        public virtual string EscapeValueQuote
        {
            get { return ValueQuote + ValueQuote; }
        }

        public virtual string QuoteValue(object value)
        {
            if (value == null)
            {
                return FormatNull();
            }

            string stringValue = value as string;
            if (stringValue != null)
            {
                return FormatString(stringValue);
            }

            if (value is char)
            {
                return FormatChar((char)value);
            }

            if (value is bool)
            {
                return FormatBool((bool)value);
            }

            if (value is Guid)
            {
                return FormatGuid((Guid)value);
            }

            if (value is DateTime)
            {
                return FormatDateTime((DateTime)value);
            }

            if (value.GetType().IsEnum)
            {
                return FormatEnum(value);
            }

            if (value is double)
            {
                return FormatDouble((double)value);
            }

            if (value is float)
            {
                return FormatFloat((float)value);
            }

            if (value is decimal)
            {
                return FormatDecimal((decimal)value);
            }

            return value.ToString();
        }

        public virtual string FormatNull()
        {
            return "NULL";
        }

        public virtual string FormatString(string value)
        {
            return ValueQuote + value.Replace(ValueQuote, EscapeValueQuote) + ValueQuote;
        }

        public virtual string FormatChar(char value)
        {
            return ValueQuote + value + ValueQuote;
        }

        public virtual string FormatBool(bool value)
        {
            return value ? 1.ToString() : 0.ToString();
        }

        public virtual string FormatGuid(Guid value)
        {
            return ValueQuote + value.ToString() + ValueQuote;
        }

        public virtual string FormatDateTime(DateTime value)
        {
            return ValueQuote + value.ToString("yyyy-MM-ddTHH:mm:ss") + ValueQuote;
        }

        public virtual string FormatEnum(object value)
        {
            return ValueQuote + value + ValueQuote;
        }

        /// <summary>
        /// Returns true is the value starts and ends with a close quote
        /// </summary>
        public virtual bool IsQuoted(string name)
        {
            // This can return true incorrectly in some cases edge cases.
            // If a string say [myname]] is passed in this is not correctly quote for MSSQL but this function will
            // return true. 
            return name.StartsWith(OpenQuote) && name.EndsWith(CloseQuote);
        }

        public virtual string QuoteCommand(string command)
        {
            return command.Replace("\'", "\'\'");
        }

        /// <summary>
        /// Returns a quoted string that has been correctly escaped
        /// </summary>
        public virtual string Quote(string name)
        {
            // Exit early if not quoting is needed
            if (!ShouldQuote(name))
            {
                return name;
            }

            string quotedName = name;
            if (!string.IsNullOrEmpty(OpenQuoteEscapeString))
            {
                quotedName = name.Replace(OpenQuote, OpenQuoteEscapeString);
            }

            // If closing quote is the same as the opening quote then no need to escape again
            if (!string.IsNullOrEmpty(CloseQuoteEscapeString))
            {
                quotedName = quotedName.Replace(CloseQuote, CloseQuoteEscapeString);
            }

            return OpenQuote + quotedName + CloseQuote;
        }

        /// <summary>
        /// Quotes a column name
        /// </summary>
        public virtual string QuoteColumnName(string columnName)
        {
            return IsQuoted(columnName) ? columnName : Quote(columnName);
        }

        /// <summary>
        /// Quote an index name
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        public virtual string QuoteIndexName(string indexName)
        {
            return IsQuoted(indexName) ? indexName : Quote(indexName);
        }

        /// <summary>
        /// Quotes a Table name
        /// </summary>
        public virtual string QuoteTableName(string tableName)
        {
            return IsQuoted(tableName) ? tableName : Quote(tableName);
        }

        /// <summary>
        /// Quotes a Sequence name
        /// </summary>
        public virtual string QuoteSequenceName(string sequenceName)
        {
            return IsQuoted(sequenceName) ? sequenceName : Quote(sequenceName);
        }

        /// <summary>
        /// Provides and unquoted, unescaped string
        /// </summary>
        public virtual string UnQuote(string quoted)
        {
            string unquoted;

            if (IsQuoted(quoted))
            {
                unquoted = quoted.Substring(1, quoted.Length - 2);
            }
            else
            {
                unquoted = quoted;
            }

            unquoted = unquoted.Replace(OpenQuoteEscapeString, OpenQuote);

            unquoted = unquoted.Replace(CloseQuoteEscapeString, CloseQuote);

            return unquoted;
        }

        private string FormatDecimal(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private string FormatFloat(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private string FormatDouble(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private bool ShouldQuote(string name)
        {
            return (!string.IsNullOrEmpty(OpenQuote) || !string.IsNullOrEmpty(CloseQuote)) &&
                   !string.IsNullOrEmpty(name);
        }
    }
}
