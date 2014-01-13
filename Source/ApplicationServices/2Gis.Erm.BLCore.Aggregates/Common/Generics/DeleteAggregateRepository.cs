using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public class DeleteAggregateRepository<T> : IDeleteAggregateRepository<T> where T : class, IEntity, IEntityKey
    {
        private readonly IFinder _finder;
        private readonly IRepository<T> _repository;

        public DeleteAggregateRepository(IFinder finder, IRepository<T> repository)
        {
            _finder = finder;
            _repository = repository;
        }

        public int Delete(long entityId)
        {
            var entity = _finder.Find<T>(x => x.Id == entityId).Single();
            _repository.Delete(entity);
            return _repository.Save();
        }
    }

    public class DeleteAggregateSecureRepository<T> : IDeleteAggregateRepository<T> where T : class, IEntity, IEntityKey, ICuratedEntity
    {
        private readonly IFinder _finder;
        private readonly ISecureRepository<T> _secureRepository;

        public DeleteAggregateSecureRepository(IFinder finder, ISecureRepository<T> secureRepository)
        {
            _finder = finder;
            _secureRepository = secureRepository;
        }

        public int Delete(long entityId)
        {
            var entity = _finder.Find<T>(x => x.Id == entityId).Single();
            _secureRepository.Delete(entity);
            return _secureRepository.Save();
        }
    }
}