using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    [DataContract]
    public class ChileBranchOfficeOrganizationUnitDomainEntityDto : BranchOfficeOrganizationUnitDomainEntityDto, IChileAdapted
    {
        [DataMember]
        public string BranchOfficeAddlRut { get; set; }
        [DataMember]
        public string RepresentativeRut { get; set; }
    }
}
