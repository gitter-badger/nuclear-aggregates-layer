using System;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public sealed class ChargeInfoDto
    {
        public long ProjectId { get; set; }
        public decimal Amount { get; set; }
        public Guid SessionId { get; set; }
    }
}