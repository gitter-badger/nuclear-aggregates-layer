using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderTerminationNoticeRequest : Request
    {
        public long OrderId { get; set; }
        public bool WithoutReason { get; set; }
        public bool TerminationBargain { get; set; }
    }
}