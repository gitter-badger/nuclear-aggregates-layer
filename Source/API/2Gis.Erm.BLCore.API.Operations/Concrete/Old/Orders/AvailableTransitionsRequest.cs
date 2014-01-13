using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class AvailableTransitionsRequest : Request
    {
        public long OrderId { get; set; }
        public long? SourceOrganizationUnitId { get; set; }
        public OrderState CurrentState { get; set; }
    }
}
