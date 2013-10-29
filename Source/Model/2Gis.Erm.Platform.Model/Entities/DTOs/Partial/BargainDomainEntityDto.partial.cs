using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class BargainDomainEntityDto
    {
        [DataMember]
        public EntityReference LegalPersonRef { get; set; }
        [DataMember]
        public EntityReference BranchOfficeOrganizationUnitRef { get; set; }

        // FIXME {all, 22.03.2013}: Добавить Timestamp в сущность Bargain
        // DONE {all, 04.10.2013}: готово
    }
}