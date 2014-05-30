using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto
{
    public class ChargesHistoryDto
    {
        public long ProjectId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public string Message { get; set; }
        public ChargesHistoryStatus Status { get; set; }
        public string Comment { get; set; }
        public Guid SessionId { get; set; }
    }
}