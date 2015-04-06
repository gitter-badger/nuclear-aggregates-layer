using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto
{
    [DataContract]
    public sealed class OrderPositionWithSchemaDto : Response
    {
        [DataMember]
        public decimal PricePerUnit { get; set; }
        [DataMember]
        public decimal PricePerUnitWithVat { get; set; }
        [DataMember]
        public string PlatformName { get; set; }
        [DataMember]
        public int? PricePositionAmount { get; set; }
        [DataMember]
        public int AmountSpecificationMode { get; set; }
        [DataMember]
        public PositionBindingObjectType LinkingObjectType { get; set; }
        [DataMember]
        public short OrderReleaseCountPlan { get; set; }
        [DataMember]
        public short OrderReleaseCountFact { get; set; }
        [DataMember]
        public decimal PricePositionCost { get; set; }
        [DataMember]
        public bool IsPositionComposite { get; set; }
        [DataMember]
        public bool IsPositionOfPlannedProvisionSalesModel { get; set; }
        [DataMember]
        public bool IsPositionCategoryBound { get; set; }
        [DataMember]
        public LinkingObjectsSchemaDto LinkingObjectsSchema { get; set; }
        [DataMember]
        public int SalesModel { get; set; }
    }
}