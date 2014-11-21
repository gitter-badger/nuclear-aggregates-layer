using System;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AdvertisementElementTemplateDomainEntityDto
    {
        [DataMember]
        public Uri IdentityServiceUrl { get; set; }
        [DataMember]
        public EntityReference DummyAdvertisementElementRef { get; set; }

        // supported file types
        public bool IsPngSupported { get; set; }
        public bool IsGifSupported { get; set; }
        public bool IsBmpSupported { get; set; }

        // supported article types
        public bool IsChmSupported { get; set; }
    }
}