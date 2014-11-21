using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    // ReSharper disable InconsistentNaming
    public class SetADPositionsValidationAsInvalidRequest : Request
    // ReSharper restore InconsistentNaming
    {
        public long OrderId { get; set; } 
    }
}