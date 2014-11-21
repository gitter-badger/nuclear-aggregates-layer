using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class LegalPersonProfileDomainEntityDto
    {
        [DataMember]
        public LegalPersonType LegalPersonType { get; set; }
    }
}