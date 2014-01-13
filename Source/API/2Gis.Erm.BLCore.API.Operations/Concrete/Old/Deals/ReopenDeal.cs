using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals
// ReSharper restore CheckNamespace
{
    public class ReopenDealRequest : Request
    {
        public long DealId { get; set; }
    }

    public class ReopenDealResponse : Response
    {
        public String Message { get; set; }
    }
}
