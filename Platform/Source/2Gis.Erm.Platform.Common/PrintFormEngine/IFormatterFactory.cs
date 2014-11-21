using System;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
	public interface IFormatterFactory
	{
        IFormatter Create(Type type, FormatType formatType, int currencyIsoCode);
	}
}