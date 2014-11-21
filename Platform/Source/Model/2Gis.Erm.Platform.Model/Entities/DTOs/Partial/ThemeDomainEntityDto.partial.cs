using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class ThemeDomainEntityDto
    {
        [DataMember]
        public ThemeTemplateCode ThemeTemplateCode { get; set; }
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }
        [DataMember]
        public string FileContentType { get; set; }
        [DataMember]
        public long FileContentLength { get; set; }
        [DataMember]
        public int OrganizationUnitCount { get; set; }
    }
}