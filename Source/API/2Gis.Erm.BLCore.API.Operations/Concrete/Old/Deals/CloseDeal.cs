using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals
// ReSharper restore CheckNamespace
{
    public sealed class CloseDealRequest : Request
    {
        public long Id { get; set; }

        public CloseDealReason CloseReason { get; set; }
        public string CloseReasonOther { get; set; }
        public string Comment { get; set; }
    }
}
