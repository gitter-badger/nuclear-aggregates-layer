using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class BillDomainEntityDto
    {
        [DataMember]
        public bool IsOrderActive { get; set; }
    }
}