using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AssociatedPositionsGroupDomainEntityDto
    {
        [DataMember]
        public bool PriceIsDeleted { get; set; }
        [DataMember]
        public bool PriceIsPublished { get; set; }
    }
}