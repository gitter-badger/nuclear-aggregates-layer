using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages
{
    public sealed class ReprocessLocalMessagesRequest : Request
    {
        public int PeriodInMinutes { get; set; }
    }
}