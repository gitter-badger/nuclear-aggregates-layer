using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class PricePositionDomainEntityDto
    {
        [DataMember]
        public bool IsRatePricePositionAvailable { get; set; }
        [DataMember]
        public bool IsPositionControlledByAmount { get; set; }
        [DataMember]
        public bool PriceIsDeleted { get; set; }
        [DataMember]
        public bool PriceIsPublished { get; set; }
        [DataMember]
        public EntityReference CurrencyRef { get; set; }
    }
}