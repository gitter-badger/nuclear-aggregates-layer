using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Caching;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Currency
{
    public class GetBaseCurrencyService : IGetBaseCurrencyService
    {
        private const string BaseCurrencyCacheKey = "ErmBaseCurrency";
        private readonly TimeSpan _baseCurrencyCacheExpiration = TimeSpan.FromMinutes(5);
        private readonly IFinder _finder;
        private readonly ICacheAdapter _cacheAdapter;

        public GetBaseCurrencyService(IFinder finder, ICacheAdapter cacheAdapter)
        {
            _finder = finder;
            _cacheAdapter = cacheAdapter;
        }

        public Platform.Model.Entities.Erm.Currency GetBaseCurrency()
        {
            var baseCurrency = _cacheAdapter.Get<Platform.Model.Entities.Erm.Currency>(BaseCurrencyCacheKey);

            if (baseCurrency == null)
            {
                var baseCurrencies = _finder.Find<Platform.Model.Entities.Erm.Currency>(x => x.IsBase && !x.IsDeleted && x.IsActive).Take(2).ToArray();

                if (baseCurrencies.Length == 0)
                {
                    throw new NotificationException(BLResources.BaseCurrencyNotFound);
                }

                if (baseCurrencies.Length > 1 && baseCurrencies[1] != null)
                {
                    throw new NotificationException(BLResources.MultipleBaseCurrencyFound);
                }

                baseCurrency = baseCurrencies[0];
                _cacheAdapter.Add(BaseCurrencyCacheKey, baseCurrency, _baseCurrencyCacheExpiration);
            }

            return baseCurrency;
        }
    }
}