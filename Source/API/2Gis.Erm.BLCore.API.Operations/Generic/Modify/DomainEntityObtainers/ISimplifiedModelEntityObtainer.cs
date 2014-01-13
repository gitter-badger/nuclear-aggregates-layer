using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers
{
    public interface ISimplifiedModelEntityObtainer<out TEntity> : ISimplifiedModelConsumer where TEntity : class, IEntityKey
    {
        TEntity ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto);
    }
}