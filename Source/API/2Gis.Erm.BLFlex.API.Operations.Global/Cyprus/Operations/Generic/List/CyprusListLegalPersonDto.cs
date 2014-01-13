using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List
{
    public class CyprusListLegalPersonDto : IListItemEntityDto<LegalPerson>, ICyprusAdapted
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        public string Tic { get; set; }
        public string Vat { get; set; }
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