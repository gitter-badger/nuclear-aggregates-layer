using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public interface IAppropriateEntityProvider<TEntity> where TEntity : class, IEntity
    {
        TEntity Get(IFindSpecification<TEntity> findSpecification);
        TDto Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification);
        IReadOnlyCollection<TEntity> Get(IFindSpecification<TEntity> findSpecification, int maxCount);
        IReadOnlyCollection<TDto> Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification, int maxCount);
    }
}