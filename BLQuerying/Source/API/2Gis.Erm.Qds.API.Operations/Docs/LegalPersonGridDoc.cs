using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class LegalPersonGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string LegalAddress { get; set; }
        public string PassportNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // relations
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string OwnerCode { get; set; }
        public string OwnerName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}