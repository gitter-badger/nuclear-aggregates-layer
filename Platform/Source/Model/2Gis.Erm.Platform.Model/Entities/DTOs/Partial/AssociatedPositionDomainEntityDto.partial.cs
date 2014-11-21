using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AssociatedPositionDomainEntityDto
    {
        [DataMember]
        public EntityReference PricePositionRef { get; set; }
        [DataMember]
        public bool PriceIsPublished { get; set; }
        [DataMember]
        public bool PriceIsDeleted { get; set; }
    }
}