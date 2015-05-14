using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export.QueryBuider
{
    // TODO {a.rechkalov, 16.08.2013}: Нужно будет объединить эту абстракцию и IExportRepository с абстракцией Read Model, т.к. у них одна и та же цель
    public interface IQueryBuilder<TEntity> where TEntity : class, IEntityKey, IEntity
    {
        IQueryable<TEntity> Create(params FindSpecification<TEntity>[] filterSpecifications);
    }
}
