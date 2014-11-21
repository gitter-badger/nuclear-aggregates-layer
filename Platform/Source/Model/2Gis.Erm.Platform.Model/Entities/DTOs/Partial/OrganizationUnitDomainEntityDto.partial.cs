using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class OrganizationUnitDomainEntityDto
    {
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }
    }
}