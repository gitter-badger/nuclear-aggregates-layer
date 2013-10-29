using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class PriceDomainEntityDto
    {
        [DataMember]
        public string Name { get; set; }
    }
}