using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public interface IAppropriateEntityProvider<TEntity> where TEntity : class, IEntity
    {
        TEntity Get(IFindSpecification<TEntity> findSpecification);
        TDto Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification);
        IReadOnlyCollection<T> Get<T>(IFindSpecification<T> findSpecification, int maxCount) where T : class, TEntity, IEntityKey;
        IReadOnlyCollection<TDto> Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification, int maxCount);
    }
}