using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices
{
    public sealed class CopyPriceRequest : Request
    {
        public long SourcePriceId { get; set; }
        public long? TargetPriceId { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? BeginDate { get; set; }
    }
}