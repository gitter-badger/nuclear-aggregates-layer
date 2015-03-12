using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class BargainGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime? BargainEndDate { get; set; }
        public BargainKind BargainKindEnum { get; set; }
        public string BargainKind { get; set; }
        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // relations
        public string LegalPersonId { get; set; }
        public string LegalPersonLegalName { get; set; }
        public string LegalPersonLegalAddress { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        //public string BranchOfficeId { get; set; }
        //public string BranchOfficeName { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}