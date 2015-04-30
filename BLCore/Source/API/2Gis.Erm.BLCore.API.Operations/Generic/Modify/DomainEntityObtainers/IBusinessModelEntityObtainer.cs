using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers
{
    public interface IBusinessModelEntityObtainer<TEntity> : IAggregateReadModel
        where TEntity : class, IEntity, IEntityKey
    {
        TEntity ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto);
    }
}