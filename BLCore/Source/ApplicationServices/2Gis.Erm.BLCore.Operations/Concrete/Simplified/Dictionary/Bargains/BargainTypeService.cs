using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Bargains;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified.Dictionary.Bargains
{
    public sealed class BargainTypeService : IBargainTypeService
    {
        private readonly IRepository<BargainType> _genericRepository;

        public BargainTypeService(IRepository<BargainType> repository)
        {
            _genericRepository = repository;
        }

        public void CreateOrUpdate(BargainType entity)
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
