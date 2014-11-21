using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals
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
