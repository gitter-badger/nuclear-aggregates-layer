using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class CheckIsBindingObjectChangeAllowedRequest: Request
    {
        public long OrderPositionId { get; set; }
        public int AdvertisementCount { get; set; }
        public bool SkipAdvertisementCountCheck { get; set; }
    }

    public sealed class CheckIsBindingObjectChangeAllowedResponse: Response
    {
        public CheckIsBindingObjectChangeAllowedResponse()
        {

        }

        public CheckIsBindingObjectChangeAllowedResponse(string format, params object[] args)
        {
            ErrorMessage = String.Format(format, args);
        }

        public string ErrorMessage { get; private set; }
        public bool IsChangeAllowed { get { return String.IsNullOrWhiteSpace(ErrorMessage); } }
        public long OrderId { get; set; }
    }
}
