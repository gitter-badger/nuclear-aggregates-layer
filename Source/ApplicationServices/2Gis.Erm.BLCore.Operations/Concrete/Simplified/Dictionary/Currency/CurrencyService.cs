using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IFinder _finder;
        private readonly IRepository<Platform.Model.Entities.Erm.Currency> _currencyGenericRepository;
        private readonly IRepository<CurrencyRate> _currencyRateGenericRepository;
        private readonly IIdentityProvider _identityProvider;

        public CurrencyService(IFinder finder, IRepository<Platform.Model.Entities.Erm.Currency> currencyRepository, IRepository<CurrencyRate> currencyRateGenericRepository, IIdentityProvider identityProvider)
        {
            _finder = finder;
            _currencyGenericRepository = currencyRepository;
            _currencyRateGenericRepository = currencyRateGenericRepository;
            _identityProvider = identityProvider;
        }

        public void Delete(Platform.Model.Entities.Erm.Currency currency)
        {
            _currencyGenericRepository.Delete(currency);
            _currencyGenericRepository.Save();
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
                _currencyGenericRepository.Add(currency);
            }
            else
            {
                _currencyGenericRepository.Update(currency);
            }

            _currencyGenericRepository.Save();
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