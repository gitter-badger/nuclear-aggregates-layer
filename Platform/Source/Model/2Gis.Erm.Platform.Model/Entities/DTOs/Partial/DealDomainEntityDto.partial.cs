using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class DealDomainEntityDto
    {
        [DataMember]
        public Guid ClientReplicationCode { get; set; }
        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }
    }
}