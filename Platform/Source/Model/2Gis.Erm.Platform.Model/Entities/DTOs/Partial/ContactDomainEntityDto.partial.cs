using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class ContactDomainEntityDto
    {
        [DataMember]
        public Guid ClientReplicationCode { get; set; }
    }
}