using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Caching;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

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
                var baseCurrencies = _finder.Find(new FindSpecification<Platform.Model.Entities.Erm.Currency>(x => x.IsBase && !x.IsDeleted && x.IsActive))
                    .Map(q => q.Take(2))
                    .Many();

                if (baseCurrencies.Count == 0)
                {
                    throw new NotificationException(BLResources.BaseCurrencyNotFound);
                }

                if (baseCurrencies.Count > 1 && baseCurrencies.Skip(1).First() != null)
                {
                    throw new NotificationException(BLResources.MultipleBaseCurrencyFound);
                }

                baseCurrency = baseCurrencies.First();
                _cacheAdapter.Add(BaseCurrencyCacheKey, baseCurrency, _baseCurrencyCacheExpiration);
            }

            return baseCurrency;
        }
    }
}