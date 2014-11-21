using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AdvertisementDomainEntityDto
    {
        [DataMember]
        public bool HasAssignedOrder { get; set; }
        [DataMember]
        public bool IsReadOnlyTemplate { get; set; }
        [DataMember]
        public bool UserDoesntHaveRightsToEditFirm { get; set; }
    }
}