using System;
using System.Collections.Generic;
using System.Globalization;

using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    // FIXME {all, 27.02.2014}: Является клоном FormatterFactory с адаптацией форматирования длинной даты для Чили
    public sealed class ChileFormatterFactory : IFormatterFactory
    {
        private static readonly IDictionary<Type, FormatType> PredefinedFormatTypes = new Dictionary<Type, FormatType>
            {
                { typeof(decimal), FormatType.Money }, 
                { typeof(DateTime), FormatType.LongDate }, 
            };

        private static readonly IDictionary<FormatType, IFormatter> Formatters = new Dictionary<FormatType, IFormatter>
            {
                { FormatType.LongDate, new StringFormatter("{0:d' de 'MMMM' de 'yyyy}") }, 
                { FormatType.ShortDate, new StringFormatter(PrintFormFieldsFormatHelper.ShortDateFormat) }, 
                { FormatType.Money, new StringFormatter("{0:C}") }, 
                { FormatType.Percents, new StringFormatter("{0:N2}%") }, 
                { FormatType.Number, new StringFormatter("{0:N2}") }, 
                { FormatType.NumberN0, new StringFormatter("{0:N0}") }, 
                // FormatType.MoneyWords formatted by custom RubleInWordsFormatter
            };

        public IFormatter Create(Type type, FormatType formatType, int currencyIsoCode)
        {
            switch (formatType)
            {
                case FormatType.Unspecified:
                {
                    // trying to detect format type based on type
                    if (PredefinedFormatTypes.TryGetValue(type, out formatType))
                    {
                        return Formatters[formatType];
                    }

                    // if we cannot detect format type, use default formatter
                    return new StringFormatter();
                }

                case FormatType.MoneyWords:
                    throw new NotImplementedException();

                default:
                    return Formatters[formatType];
            }
        }

        private sealed class StringFormatter : IFormatter
        {
            private readonly string _format;

            public StringFormatter() : this("{0}") 
            {
            }

            public StringFormatter(string format)
            {
                _format = format;
            }

            public string Format(object data)
            {
                return string.Format(CultureInfo.CurrentCulture, _format, data);
            }
        }
    }
}