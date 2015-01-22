using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderAdditionalAgreementRequest : Request
    {
        public long OrderId { get; set; }
        public PrintAdditionalAgreementTarget PrintType { get; set; } 
    }
}