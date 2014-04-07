using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public abstract class FormatterFactoryBase : IFormatterFactory
    {
        private readonly IDictionary<Type, FormatType> _predefinedFormatTypes;
        private readonly IDictionary<FormatType, IFormatter> _formatters;
        private readonly IDictionary<int, IFormatter> _moneyFormatters;
        private readonly IDictionary<int, IFormatter> _moneyCapitalizedFormatters;

        protected FormatterFactoryBase()
        {
            _formatters = new Dictionary<FormatType, IFormatter>();
            _predefinedFormatTypes = new Dictionary<Type, FormatType>();
            _moneyFormatters = new Dictionary<int, IFormatter>();
            _moneyCapitalizedFormatters = new Dictionary<int, IFormatter>();
        }

        public IFormatter Create(Type type, FormatType formatType, int currencyIsoCode)
        {
            switch (formatType)
            {
                case FormatType.Unspecified:
                    return FormatByDataType(type);

                case FormatType.MoneyWords:
                    return FormatMoneyWords(currencyIsoCode);

                case FormatType.MoneyWordsUpperStart:
                    return FormatMoneyWordsWithCapitalStart(currencyIsoCode);

                default:
                    return _formatters[formatType];
            }
        }

        protected void SetMoneyWordsFormatter(int currencyIsoCode, IFormatter formatter)
        {
            _moneyFormatters.Add(currencyIsoCode, formatter);
            _moneyCapitalizedFormatters.Add(currencyIsoCode, new FirstLetterCapitalizer(formatter));
        }

        protected void SetFormat(FormatType formatType, string format)
        {
            _formatters.Add(formatType, new StringFormatter(format));
        }

        protected void SetFormat(FormatType formatType, IFormatter formatter)
        {
            _formatters.Add(formatType, formatter);
        }

        protected void SetTypeFormat(Type type, FormatType formatType)
        {
            _predefinedFormatTypes.Add(type, formatType);
        }

        private IFormatter FormatMoneyWordsWithCapitalStart(int currencyIsoCode)
        {
            IFormatter formatter;
            if (_moneyCapitalizedFormatters.TryGetValue(currencyIsoCode, out formatter))
            {
                return formatter;
            }

            throw new NotImplementedException();
        }

        private IFormatter FormatMoneyWords(int currencyIsoCode)
        {
            IFormatter formatter;
            if (_moneyFormatters.TryGetValue(currencyIsoCode, out formatter))
            {
                return formatter;
            }

            throw new NotImplementedException();
        }

        private IFormatter FormatByDataType(Type type)
        {
            FormatType formatType;
            IFormatter formatter;
            if (_predefinedFormatTypes.TryGetValue(type, out formatType) && _formatters.TryGetValue(formatType, out formatter))
            {
                return formatter;
            }

            return new StringFormatter();
        }
    }
}