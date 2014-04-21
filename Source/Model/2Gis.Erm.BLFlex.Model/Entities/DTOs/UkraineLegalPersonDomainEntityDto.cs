using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    [DataContract]
    public class UkraineLegalPersonDomainEntityDto : LegalPersonDomainEntityDto, IUkraineAdapted
    {
        [DataMember]
        public string Egrpou { get; set; }
        [DataMember]
        public TaxationType TaxationType { get; set; }
    }
}
