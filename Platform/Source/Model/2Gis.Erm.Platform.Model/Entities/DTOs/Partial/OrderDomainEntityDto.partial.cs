using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class OrderDomainEntityDto
    {
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public EntityReference ClientRef { get; set; }
        [DataMember]
        public bool HasAnyOrderPosition { get; set; }
        [DataMember]
        public bool HasDestOrganizationUnitPublishedPrice { get; set; }
        [DataMember]
        public long? DealCurrencyId { get; set; }
        [DataMember]
        public OrderState PreviousWorkflowStepId { get; set; }
        [DataMember]
        public bool DiscountPercentChecked { get; set; }
        [DataMember]
        public EntityReference InspectorRef { get; set; }
        [DataMember]
        public string Platform { get; set; }
        [DataMember]
        public bool CanSwitchToAccount { get; set; }
        [DataMember]
        public bool ShowRegionalAttributes { get; set; }
    }
}