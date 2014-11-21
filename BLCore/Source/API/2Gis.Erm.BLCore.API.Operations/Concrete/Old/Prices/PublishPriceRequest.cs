using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices
{
    public sealed class PublishPriceRequest : Request
    {
        public long PriceId { get; set; }
        public long OrganizarionUnitId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime PublishDate { get; set; }
    }
}