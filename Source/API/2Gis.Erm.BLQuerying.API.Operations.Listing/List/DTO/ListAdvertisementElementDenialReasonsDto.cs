using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementElementDenialReasonsDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long AdvertisementElementId { get; set; }
        public long DenialReasonId { get; set; }
        public string DenialReasonName { get; set; }
        public string DenialReasonType { get; set; }
        public string Comment { get; set; }
        public bool Checked { get; set; }
        public bool IsActive { get; set; }
    }
}