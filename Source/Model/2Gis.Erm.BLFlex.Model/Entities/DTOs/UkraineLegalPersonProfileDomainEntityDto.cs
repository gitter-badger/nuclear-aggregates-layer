using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    [DataContract]
    public sealed class UkraineLegalPersonProfileDomainEntityDto : LegalPersonProfileDomainEntityDto, IUkraineAdapted
    {
        public string Mfo { get; set; }
    }
}
