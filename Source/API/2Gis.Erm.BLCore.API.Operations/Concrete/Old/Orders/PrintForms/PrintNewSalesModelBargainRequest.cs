using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintNewSalesModelBargainRequest : Request
    {
        public long? BargainId { get; set; }
        public long? OrderId { get; set; }
        public long? LegalPersonProfileId { get; set; }
    }
}
