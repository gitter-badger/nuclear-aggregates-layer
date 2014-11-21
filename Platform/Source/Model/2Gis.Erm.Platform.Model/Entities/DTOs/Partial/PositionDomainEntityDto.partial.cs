using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class PositionDomainEntityDto
    {
        [DataMember]
        public bool IsPublished { get; set; }
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }
        [DataMember]
        public bool RestrictChildPositionPlatformsCanBeChanged { get; set; }
        [DataMember]
        public bool IsReadOnlyTemplate { get; set; }
    }
}