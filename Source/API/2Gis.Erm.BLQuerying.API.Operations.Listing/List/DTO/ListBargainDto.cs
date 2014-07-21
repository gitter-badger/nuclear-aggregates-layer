using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBargainDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? BargainEndDate { get; set; }
        public BargainKind BargainKindEnum { get; set; }
        public string BargainKind { get; set; }
        public long CustomerLegalPersonId { get; set; }
        public long BranchOfficeId { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
        public string Number { get; set; }
        public string CustomerLegalPersonLegalName { get; set; }
        public string BranchOfficeName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long OwnerCode { get; set; }
        public string LegalAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}