using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class DeniedPositionDomainEntityDto
    {
        [DataMember]
        public bool IsPricePublished { get; set; }
    }
}