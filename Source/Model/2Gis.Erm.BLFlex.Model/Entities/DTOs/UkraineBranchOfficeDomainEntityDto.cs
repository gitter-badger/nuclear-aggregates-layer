using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    [DataContract]
    public class UkraineBranchOfficeDomainEntityDto : BranchOfficeDomainEntityDto, IUkraineAdapted
    {
        [DataMember]
        public string Ipn { get; set; }
    }
}
