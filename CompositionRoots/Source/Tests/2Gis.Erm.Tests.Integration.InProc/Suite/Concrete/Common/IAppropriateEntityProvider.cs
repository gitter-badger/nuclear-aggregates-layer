using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public interface IAppropriateEntityProvider<TEntity> where TEntity : class, IEntity
    {
        TEntity Get(FindSpecification<TEntity> findSpecification);
        TDto Get<TDto>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TDto> selectSpecification);
        IReadOnlyCollection<T> Get<T>(FindSpecification<T> findSpecification, int maxCount) where T : class, TEntity, IEntityKey;
        IReadOnlyCollection<TDto> Get<TDto>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TDto> selectSpecification, int maxCount);
    }
}