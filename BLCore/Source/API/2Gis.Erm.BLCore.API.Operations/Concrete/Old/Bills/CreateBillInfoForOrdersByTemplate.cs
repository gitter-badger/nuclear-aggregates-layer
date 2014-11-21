using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills
{
    public sealed class CreateBillInfoForOrdersByTemplateRequest : Request
    {
        public CreateBillInfo[] CreateBillInfosTemplate { get; set; }
        public long[] OrderIds { get; set; }
    }

    public sealed class CreateBillInfoForOrdersByTemplateResponse : Response
    {
        public Tuple<long, CreateBillInfo[]>[] OrdersCreateBillInfos { get; set; }
    }
}
