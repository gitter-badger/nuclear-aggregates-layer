using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.DAL;

using NuClear.Storage;
using DoubleGis.Erm.Platform.DAL.Specifications;

using NuClear.Model.Common.Entities.Aspects;

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
            var entity = _finder.FindOne<T>(Specs.Find.ById<T>(entityId));
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
            var entity = _finder.FindOne(Specs.Find.ById<T>(entityId));
            entity.IsActive = true;
            _secureRepository.Update(entity);
            return _secureRepository.Save();
        }
    }
}
