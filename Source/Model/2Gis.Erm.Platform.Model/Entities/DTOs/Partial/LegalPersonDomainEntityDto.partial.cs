using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class LegalPersonDomainEntityDto
    {
        [DataMember]
        public string BusinessmanInn { get; set; }
        [DataMember]
        public bool HasProfiles { get; set; }
    }
}