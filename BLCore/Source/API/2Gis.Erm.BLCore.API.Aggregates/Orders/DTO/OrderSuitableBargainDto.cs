using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public sealed class OrderSuitableBargainDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime? EndDate { get; set; }
        public BargainKind BargainKind { get; set; }
    }
}