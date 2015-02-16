using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintLetterOfGuaranteeRequest : Request
    {
        public long OrderId { get; set; }
        public bool IsChangingAdvMaterial { get; set; }
    }
}
