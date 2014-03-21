using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLegalPersonDto : IRussiaAdapted, IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string LegalAddress { get; set; }
        public string PassportNumber { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? FirmId { get; set; }
        public long OwnerCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public string OwnerName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}