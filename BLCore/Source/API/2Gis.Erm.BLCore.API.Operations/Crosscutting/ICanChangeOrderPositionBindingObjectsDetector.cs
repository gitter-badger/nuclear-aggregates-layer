using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Crosscutting
{
    public interface ICanChangeOrderPositionBindingObjectsDetector : IInvariantSafeCrosscuttingService
    {
        bool CanChange(
            OrderState orderWorkflowState,
            PositionBindingObjectType orderPositionBindingObject,
            bool skipAdvertisementCountCheck,
            int? actualOrderPositionAdvertisementLinksCount,
            int? targetOrderPositionAdvertisementLinksCount,
            out string report);
    }
}