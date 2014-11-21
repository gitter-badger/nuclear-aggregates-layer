using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class FirmContactDomainEntityDto
    {
        [DataMember]
        public bool IsFirmAddressDeleted { get; set; }
        [DataMember]
        public bool IsFirmDeleted { get; set; }
        [DataMember]
        public bool IsFirmAddressActive { get; set; }
        [DataMember]
        public bool IsFirmActive { get; set; }
        [DataMember]
        public bool FirmAddressClosedForAscertainment { get; set; }
        [DataMember]
        public bool FirmClosedForAscertainment { get; set; }
    }
}