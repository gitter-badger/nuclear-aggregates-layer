using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    public interface IOwnerValidator : IInvariantSafeCrosscuttingService
    {
        void CheckIsNotReserve<TEntity>(long entityId) where TEntity : class, IEntityKey, ICuratedEntity, IEntity;
    }
}