using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class ViewOrderPositionRequest : Request
    {
        public long? OrderPositionId { get; set; }
        public long OrderId { get; set; }
        public long PricePositionId { get; set; }

        /// <summary>
        /// Неактивные и удаленные адреса не появляются при нормальном редактировании позиции заказа,
        /// но должны присутствовать в режиме смены объектов привязки.
        /// </summary>
        public bool IncludeHidden { get; set; }
    }

    [DataContract]
    public sealed class ViewOrderPositionResponse : Response
    {
        [DataMember]
        public decimal PricePerUnit { get; set; }
        [DataMember]
        public decimal VatRatio { get; set; }
        [DataMember]
        public bool IsBudget { get; set; }
        [DataMember]
        public string PlatformName { get; set; }
        [DataMember]
        public int? PricePositionAmount { get; set; }
        [DataMember]
        public int AmountSpecificationMode { get; set; }
        [DataMember]
        public short OrderReleaseCountPlan { get; set; }
        [DataMember]
        public short OrderReleaseCountFact { get; set; }
        [DataMember]
        public decimal PricePositionCost { get; set; }
        [DataMember]
        public bool IsPositionComposite { get; set; }

        [DataMember]
        public LinkingObjectsSchemaDto LinkingObjectsSchema { get; set; }
    }

    [DataContract]
    public sealed class LinkingObjectsSchemaDto
    {
        [DataMember]
        public IEnumerable<WarningDto> Warnings { get; set; }
        [DataMember]
        public IEnumerable<CategoryDto> FirmCategories { get; set; }
        [DataMember]
        public IEnumerable<CategoryDto> AdditionalCategories { get; set; }
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
            public bool IsLinkingObjectOfSingleType { get; set; }
        }
    }
}
