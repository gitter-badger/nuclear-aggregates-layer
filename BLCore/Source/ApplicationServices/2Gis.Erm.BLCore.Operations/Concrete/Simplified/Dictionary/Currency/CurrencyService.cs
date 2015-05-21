using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IFinder _finder;
        private readonly IRepository<Platform.Model.Entities.Erm.Currency> _currencyGenericRepository;
        private readonly IRepository<CurrencyRate> _currencyRateGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public CurrencyService(IFinder finder,
                               IRepository<Platform.Model.Entities.Erm.Currency> currencyRepository,
                               IRepository<CurrencyRate> currencyRateGenericRepository,
                               IIdentityProvider identityProvider,
                               IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _currencyGenericRepository = currencyRepository;
            _currencyRateGenericRepository = currencyRateGenericRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Delete(Platform.Model.Entities.Erm.Currency currency)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Platform.Model.Entities.Erm.Currency>())
            {
            _currencyGenericRepository.Delete(currency);
            _currencyGenericRepository.Save();

                scope.Deleted(currency)
                     .Complete();
        }
        }

        public CurrencyWithRelationsDto GetCurrencyWithRelations(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Platform.Model.Entities.Erm.Currency>(entityId) 
                                    && Specs.Find.ActiveAndNotDeleted<Platform.Model.Entities.Erm.Currency>())
                          .Select(currency => new CurrencyWithRelationsDto
                              {
                                  Currency = currency,
                                  RelatedCountryName = currency.Countries.FirstOrDefault(y => !y.IsDeleted).Name,
                                  RelatedCurrencyRateCreateDate = (DateTime?)currency.CurrencyRates.FirstOrDefault(y => !y.IsDeleted).CreatedOn,
                              })
                          .SingleOrDefault();
        }

        public void CreateOrUpdate(Platform.Model.Entities.Erm.Currency currency)
        {
            if (currency.IsBase)
            {
                var isDuplicatedCurrency = _finder.Find<Platform.Model.Entities.Erm.Currency>(x => !x.IsDeleted && x.IsActive && x.IsBase && x.Id != currency.Id).Any();
                if (isDuplicatedCurrency)
                {
                    throw new NotificationException(BLResources.CurrencyController_BaseCurrencyIsExists);
                }
            }

            if (currency.IsNew())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Platform.Model.Entities.Erm.Currency>())
                {
                _currencyGenericRepository.Add(currency);
                    scope.Added<Platform.Model.Entities.Erm.Currency>(currency.Id);
                    _currencyGenericRepository.Save();

                    scope.Complete();
                }
            }
            else
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Platform.Model.Entities.Erm.Currency>())
                {
                    _currencyGenericRepository.Update(currency);
                    scope.Updated<Platform.Model.Entities.Erm.Currency>(currency.Id);
                    _currencyGenericRepository.Save();

                    scope.Complete();
                }
            }
        }

        public void SetCurrencyRate(CurrencyRate currencyRate)
        {
            var isCurrencyRateExistForDate = _finder.Find<Platform.Model.Entities.Erm.Currency>(x => x.Id == currencyRate.CurrencyId).SelectMany(x => x.CurrencyRates).Where(x => !x.IsDeleted).Any(x => x.CreatedOn == currencyRate.CreatedOn);
            if (isCurrencyRateExistForDate)
            {
                var currency = _finder.Find<Platform.Model.Entities.Erm.Currency>(x => x.Id == currencyRate.CurrencyId).Single();
                throw new NotificationException(string.Format(CultureInfo.InvariantCulture, BLResources.CurrencyRateForDateAlreadyExist, currency.Name, currencyRate.CreatedOn.ToShortDateString()));
            }

            if (currencyRate.IsNew())
            {
                _identityProvider.SetFor(currencyRate);
                _currencyRateGenericRepository.Add(currencyRate);
            }
            else
            {
                _currencyRateGenericRepository.Update(currencyRate);
            }

            _currencyRateGenericRepository.Save();
        }
    }
}