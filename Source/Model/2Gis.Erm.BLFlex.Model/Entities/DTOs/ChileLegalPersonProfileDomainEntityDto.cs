using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    // FIXME {all, 14.02.2014}: Временное решение, до начала работы IPartable на сущностях IDomainEntityDto
    [DataContract]
    public sealed class ChileLegalPersonProfileDomainEntityDto : LegalPersonProfileDomainEntityDto, IChileAdapted
    {
        public AccountType AccountType { get; set; }  
        public EntityReference BankRef { get; set; }
        public string RepresentativeRut { get; set; }
        public DateTime? RepresentativeDocumentIssuedOn { get; set; }
        public string RepresentativeDocumentIssuedBy { get; set; }
    }
}
