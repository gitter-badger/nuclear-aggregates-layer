namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO
{
    public class OrderPositionChargeInfo
    {
        public long FirmId { get; set; }
        public long PositionId { get; set; }
        public long CategoryId { get; set; }

        public override string ToString()
        {
            return string.Format("FirmId: {0}, PositionId: {1}, CategoryId: {2}", FirmId, PositionId, CategoryId);
        }
    }
}