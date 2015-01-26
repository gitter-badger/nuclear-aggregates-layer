using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities
{
    public interface IOrderProcessingRequestStateAspect : IAspect
    {
        OrderProcessingRequestState State { get; set; }
    }
}