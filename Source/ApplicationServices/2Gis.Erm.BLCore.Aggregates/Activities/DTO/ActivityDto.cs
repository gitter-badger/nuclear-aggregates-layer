using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.DTO
{
    public class ActivityDto
    {
        public long Id { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? ContactId { get; set; }
        public string ContactName { get; set; }
        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        public long? DealId { get; set; }
        public string DealName { get; set; }
        public ActivityStatus Status { get; set; }
    }
}