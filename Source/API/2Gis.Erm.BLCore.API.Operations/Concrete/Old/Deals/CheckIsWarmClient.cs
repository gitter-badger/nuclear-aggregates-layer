using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals
// ReSharper restore CheckNamespace
{
    public class CheckIsWarmClientRequest : Request
    {
        public long ClientId { get; set; }
    }

    public class CheckIsWarmClientResponse : Response
    {
        public Boolean IsWarmClient { get; set; }
        public String TaskDescription { get; set; }
        public DateTime? TaskActualEnd { get; set; }
    }

}
