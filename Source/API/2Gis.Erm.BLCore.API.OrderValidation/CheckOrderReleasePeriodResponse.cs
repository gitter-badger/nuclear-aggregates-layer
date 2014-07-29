using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class CheckOrderReleasePeriodResponse : Response
    {
        public bool Success { get; set; }
        public OrderValidationMessage Message { get; set; }
    }
}
