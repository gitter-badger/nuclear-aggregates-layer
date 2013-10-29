using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class LockDomainEntityDto
    {
        [DataMember]
        public EntityReference BranchOfficeOrganizationUnitRef { get; set; }
        [DataMember]
        public EntityReference LegalPersonRef { get; set; }
        [DataMember]
        public EntityReference OrderRef { get; set; }
    }
}