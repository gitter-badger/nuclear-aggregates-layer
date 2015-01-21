using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class ActualizeOrderReleaseWithdrawalsRequest : Request
    {
        public Order Order { get; set; }
    }
}