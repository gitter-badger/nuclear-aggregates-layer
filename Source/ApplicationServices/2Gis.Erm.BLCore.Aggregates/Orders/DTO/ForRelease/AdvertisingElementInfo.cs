using System;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease
{
    public sealed class AdvertisingElementInfo
    {
        public long Id { get; set; }
        public long? FileId { get; set; }
        public string Text { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
        public int ExportCode { get; set; }
    }
}