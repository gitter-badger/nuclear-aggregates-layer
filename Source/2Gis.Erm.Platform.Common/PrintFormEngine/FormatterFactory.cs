using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
	public sealed class FormatterFactory : IFormatterFactory
	{
		private static readonly IDictionary<Type, FormatType> PredefinedFormatTypes = new Dictionary<Type, FormatType>
		{
			{ typeof(Decimal), FormatType.Money },
			{ typeof(DateTime), FormatType.LongDate },
		};

		private static readonly IDictionary<FormatType, IFormatter> Formatters = new Dictionary<FormatType, IFormatter>
		{
			{ FormatType.LongDate, new StringFormatter(PrintFormFieldsFormatHelper.LongDateFormat) },
			{ FormatType.ShortDate, new StringFormatter(PrintFormFieldsFormatHelper.ShortDateFormat) },
			{ FormatType.Money, new StringFormatter("{0:C}") },
			// FormatType.MoneyWords formatted by custom RubleInWordsFormatter
			{ FormatType.Percents, new StringFormatter("{0:N2}%") },
			{ FormatType.Number, new StringFormatter("{0:N2}") },
			{ FormatType.NumberN0, new StringFormatter("{0:N0}") },
		};

		public IFormatter Create(Type type, FormatType formatType, int currencyIsoCode)
		{
			switch (formatType)
			{
				case FormatType.Unspecified:
					{
						// trying to detect format type based on type
						if (PredefinedFormatTypes.TryGetValue(type, out formatType))
							return Formatters[formatType];

						// if we cannot detect format type, use default formatter
						return new StringFormatter();
					}

				case FormatType.MoneyWords:
					switch (currencyIsoCode)
					{
						case 643:
							return new RublesInWordsFormatter();
						default:
							throw new NotImplementedException();
					}

				default:
					return Formatters[formatType];
			}
		}
	}
}