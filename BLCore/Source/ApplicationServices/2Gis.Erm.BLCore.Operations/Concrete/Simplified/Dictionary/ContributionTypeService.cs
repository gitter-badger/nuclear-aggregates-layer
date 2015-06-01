using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary
{
    public class ContributionTypeService : IContributionTypeService
    {
        private readonly IRepository<ContributionType> _genericRepository;

        public ContributionTypeService(IRepository<ContributionType> repository)
        {
            _genericRepository = repository;
        }

        public void CreateOrUpdate(ContributionType entity)
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
