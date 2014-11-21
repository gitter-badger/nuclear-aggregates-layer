using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public class CreateLockDetailDto
    {
        public Lock Lock { get; set; }
        public long PriceId { get; set; }
        public long OrderPositionId { get; set; }
        public decimal Amount { get; set; }
        public Guid ChargeSessionId { get; set; }
    }
}