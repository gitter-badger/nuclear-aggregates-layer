using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class ChangeBindingObjectsRequest: Request
    {
        public long OrderPositionId { get; set; }
        public AdvertisementDescriptor[] Advertisements { get; set; }
    }
}