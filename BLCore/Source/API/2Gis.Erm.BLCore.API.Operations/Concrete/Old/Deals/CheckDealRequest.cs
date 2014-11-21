using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals
// ReSharper restore CheckNamespace
{
    public sealed class CheckDealRequest : Request
    {
        public long DealId { get; set; }
        public long CurrencyId { get; set; }
    }
}
