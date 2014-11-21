using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class BranchOfficeOrganizationUnitDomainEntityDto
    {
        [DataMember]
        public long BranchOfficeAddlId { get; set; }
        [DataMember]
        public string BranchOfficeAddlInn { get; set; }
        [DataMember]
        public string BranchOfficeAddlLegalAddress { get; set; }
        [DataMember]
        public string BranchOfficeAddlName { get; set; }
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }
        [DataMember]
        public string BranchOfficeAddlIc { get; set; }
    }
}