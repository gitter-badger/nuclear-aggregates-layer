using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class FirmAddressDomainEntityDto
    {
        [DataMember]
        public bool IsFirmActive { get; set; }
        [DataMember]
        public bool IsFirmDeleted { get; set; }
        [DataMember]
        public bool FirmClosedForAscertainment { get; set; }
    }
}