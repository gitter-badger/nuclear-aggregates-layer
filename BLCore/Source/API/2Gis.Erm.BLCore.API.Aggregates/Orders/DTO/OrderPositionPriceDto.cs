﻿namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public class OrderPositionPriceDto
    {
        public decimal PricePerUnit { get; set; }
        public decimal VatRatio { get; set; }
        public decimal CategoryRate { get; set; }
    }
}