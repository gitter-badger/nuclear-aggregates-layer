using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AccountDetailDomainEntityDto
    {
        [DataMember]
        public bool OwnerCanBeChanged { get; set; }
    }
}