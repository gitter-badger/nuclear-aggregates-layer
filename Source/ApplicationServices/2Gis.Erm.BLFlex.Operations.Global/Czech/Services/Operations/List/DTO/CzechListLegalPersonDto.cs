using System;

using DoubleGis.Erm.BL.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Core.Services.Operations.Concrete.List.DTO
{
    public class CzechListLegalPersonDto : IListItemEntityDto<LegalPerson>, ICzechAdapted
    {
        public long Id { get; set; }
        public string LegalName { get; set; }
        public string Ic { get; set; }
        public string Dic { get; set; }
        public string LegalAddress { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
    }
}
