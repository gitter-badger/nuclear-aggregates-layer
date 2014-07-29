using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class CheckOrdersReadinessForReleaseRequest : Request
    {
        public TimePeriod Period { get; set; }
        public long? OrganizationUnitId { get; set; }
        public long? OwnerId { get; set; }
        public bool IncludeOwnerDescendants { get; set; }
        public bool CheckAccountBalance { get; set; }
    }
}