using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLocalMessageDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public IntegrationTypeImport IntegrationTypeImport { get; set; }
        public IntegrationTypeExport IntegrationTypeExport { get; set; }
        public string IntegrationType { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public LocalMessageStatus StatusEnum { get; set; }
        public string Status { get; set; }
        public IntegrationSystem SenderSystemEnum { get; set; }
        public string SenderSystem { get; set; }
        public IntegrationSystem ReceiverSystemEnum { get; set; }
        public string ReceiverSystem { get; set; }
    }
}