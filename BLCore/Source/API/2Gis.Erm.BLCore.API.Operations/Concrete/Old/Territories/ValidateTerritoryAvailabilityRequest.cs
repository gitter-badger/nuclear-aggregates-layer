using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories
{
    public sealed class ValidateTerritoryAvailabilityRequest : Request
    {
        public long TerritoryId { get; set; }
    }
}