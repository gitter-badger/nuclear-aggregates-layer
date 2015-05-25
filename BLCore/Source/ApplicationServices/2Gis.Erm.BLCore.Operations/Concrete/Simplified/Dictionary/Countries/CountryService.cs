using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Countries;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Countries
{
    public sealed class CountryService : ICountryService
    {
        private readonly IRepository<Country> _countryGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CountryService(IRepository<Country> countryGenericRepository, IOperationScopeFactory scopeFactory)
        {
            _countryGenericRepository = countryGenericRepository;
            _scopeFactory = scopeFactory;
        }

        public void CreateOrUpdate(Country country)
        {
            if (country.IsNew())
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Country>())
                {
                    _countryGenericRepository.Add(country);
                    scope.Added<Country>(country.Id);
                    _countryGenericRepository.Save();

                    scope.Complete();
                }
                
            }
            else
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Country>())
                {
                    _countryGenericRepository.Update(country);
                    scope.Updated<Country>(country.Id);
                    _countryGenericRepository.Save();

                    scope.Complete();
                }
            }
        }
    }
}
