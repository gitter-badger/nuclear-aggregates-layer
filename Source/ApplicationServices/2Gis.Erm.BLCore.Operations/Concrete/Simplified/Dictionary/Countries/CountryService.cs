using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Countries;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Countries
{
    public sealed class CountryService : ICountryService
    {
        private readonly IRepository<Country> _genericRepository;

        public CountryService(IRepository<Country> repository)
        {
            _genericRepository = repository;
        }

        public void CreateOrUpdate(Country entity)
        {
            if (entity.IsNew())
            {
                _genericRepository.Add(entity);
            }
            else
            {
                _genericRepository.Update(entity);
            }

            _genericRepository.Save();
        }
    }
}
