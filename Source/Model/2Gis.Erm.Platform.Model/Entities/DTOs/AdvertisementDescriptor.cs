using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public sealed class AdvertisementDescriptor
    {
        [DataMember]
        public long PositionId { get; set; }

        [DataMember]
        public long? AdvertisementId { get; set; }

        [DataMember]
        public long? DummyAdvertisementId { get; set; }

        [DataMember]
        public string AdvertisementName { get; set; }

        [DataMember]
        public long? CategoryId { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public long? FirmAddressId { get; set; }

        [DataMember]
        public string FirmAddress { get; set; }

        [DataMember]
        public long? ThemeId { get; set; }

        [DataMember]
        public string ThemeName { get; set; }

        [DataMember]
        public bool IsAdvertisementRequired { get; set; }
    }
}