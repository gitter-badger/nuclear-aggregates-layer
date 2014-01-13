using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains
{
    public class GetOrderBargainResult
    {
        public long BargainId { get; set; }
        public string BargainNumber { get; set; }
        public Nullable<DateTime> BargainClosedOn { get; set; }
    }
}
