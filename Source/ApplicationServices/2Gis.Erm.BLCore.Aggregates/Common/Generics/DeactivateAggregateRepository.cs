using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DeactivateAggregateRepository<T> : IDeactivateAggregateRepository<T> where T : class, IEntity, IEntityKey, IDeactivatableEntity
    {
        private readonly IFinder _finder;
        private readonly IRepository<T> _repository;

        public DeactivateAggregateRepository(IFinder finder, IRepository<T> repository)
        {
            _finder = finder;
            _repository = repository;
        }

        public int Deactivate(long entityId)
        {
            var entity = _finder.Find<T>(x => x.Id == entityId).Single();
            entity.IsActive = false;
            _repository.Update(entity);
            return _repository.Save();
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DeactivateAggregateSecureRepository<T> : IDeactivateAggregateRepository<T> where T : class, IEntity, IEntityKey, IDeactivatableEntity, ICuratedEntity
    {
        private readonly IFinder _finder;
        private readonly ISecureRepository<T> _secureRepository;

        public DeactivateAggregateSecureRepository(IFinder finder, ISecureRepository<T> secureRepository)
        {
            _finder = finder;
            _secureRepository = secureRepository;
        }

        public int Deactivate(long entityId)
        {
            var entity = _finder.Find<T>(x => x.Id == entityId).Single();
            entity.IsActive = false;
            _secureRepository.Update(entity);
            return _secureRepository.Save();
        }
    }
}