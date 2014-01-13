using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ActivateAggregateRepository<T> : IActivateAggregateRepository<T> where T : class, IEntity, IEntityKey, IDeactivatableEntity
    {
        private readonly IFinder _finder;
        private readonly IRepository<T> _repository;

        public ActivateAggregateRepository(IFinder finder, IRepository<T> repository)
        {
            _finder = finder;
            _repository = repository;
        }

        public int Activate(long entityId)
        {
            var entity = _finder.Find<T>(x => x.Id == entityId).Single();
            entity.IsActive = true;
            _repository.Update(entity);
            return _repository.Save();
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ActivateAggregateSecureRepository<T> : IActivateAggregateRepository<T> where T : class, IEntity, IEntityKey, IDeactivatableEntity, ICuratedEntity
    {
        private readonly IFinder _finder;
        private readonly ISecureRepository<T> _secureRepository;

        public ActivateAggregateSecureRepository(IFinder finder, ISecureRepository<T> secureRepository)
        {
            _finder = finder;
            _secureRepository = secureRepository;
        }

        public int Activate(long entityId)
        {
            var entity = _finder.Find<T>(x => x.Id == entityId).Single();
            entity.IsActive = true;
            _secureRepository.Update(entity);
            return _secureRepository.Save();
        }
    }
}
