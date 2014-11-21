using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers
{
    public interface IBusinessModelEntityObtainer<TEntity> : IAggregateReadModel
        where TEntity : class, IEntity, IEntityKey
    {
        TEntity ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto);
    }
}