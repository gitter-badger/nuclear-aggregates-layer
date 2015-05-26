using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto
{
    [DataContract]
    public sealed class LinkingObjectsSchemaDto
    {
        [DataMember]
        public IEnumerable<WarningDto> Warnings { get; set; }
        [DataMember]
        public IEnumerable<CategoryDto> FirmCategories { get; set; }
        [DataMember]
        public IEnumerable<PositionDto> Positions { get; set; }
        [DataMember]
        public IEnumerable<FirmAddressDto> FirmAddresses { get; set; }
        [DataMember]
        public IEnumerable<ThemeDto> Themes { get; set; }

        [DataContract]
        public sealed class WarningDto
        {
            [DataMember]
            public string Text { get; set; }
        }

        [DataContract]
        public sealed class FirmAddressDto
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public string Address { get; set; }
            [DataMember]
            public bool IsHidden { get; set; }
            [DataMember]
            public bool IsLocatedOnTheMap { get; set; }
            [DataMember]
            public bool IsDeleted { get; set; }

            [DataMember]
            public IEnumerable<long> Categories { get; set; }
        }

        [DataContract]
        public sealed class ThemeDto
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public string Name { get; set; }
        }

        [DataContract]
        public sealed class CategoryDto
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Level { get; set; }
        }

        [DataContract]
        public sealed class PositionDto
        {
            [DataMember]
            public long Id { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string LinkingObjectType { get; set; }
            [DataMember]
            public long? AdvertisementTemplateId { get; set; }
            [DataMember]
            public long? DummyAdvertisementId { get; set; }
            [DataMember]
            public IEnumerable<CategoryDto> AvailableCategories { get; set; }

            [DataMember]
            public bool IsLinkingObjectOfSingleType { get; set; }
            [DataMember]
            public int PositionsGroup { get; set; }
            [DataMember]
            public bool AlwaysChecked { get; set; }
        }
    }
}