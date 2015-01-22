using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers
{
    public interface ISimplifiedModelEntityObtainer<out TEntity> : ISimplifiedModelConsumer where TEntity : class, IEntityKey
    {
        TEntity ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto);
    }
}