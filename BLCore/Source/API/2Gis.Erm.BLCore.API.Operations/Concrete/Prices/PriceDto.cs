using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public sealed class PriceDto
    {
        public long OrganizationUnitId { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime CreateDate { get; set; }
    } 
}