using DoubleGis.Erm.Platform.Common.Crosscutting;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    public interface IOwnerValidator : IInvariantSafeCrosscuttingService
    {
        void CheckIsNotReserve(ICuratedEntity entity);
    }
}