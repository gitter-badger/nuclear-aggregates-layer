using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLocalMessageDto : IListItemEntityDto<LocalMessage>
    {
        public long Id { get; set; }
        public string IntegrationType { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Status { get; set; }
        public string SenderSystem { get; set; }
        public string ReceiverSystem { get; set; }
    }
}