using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class OrderPositionDomainEntityDto
    {
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public bool IsComposite { get; set; }
        [DataMember]
        public long OrganizationUnitId { get; set; }
        [DataMember]
        public DateTime PeriodStartDate { get; set; }
        [DataMember]
        public DateTime PeriodEndDate { get; set; }
        [DataMember]
        public long PriceId { get; set; }
        [DataMember]
        public long? OrderFirmId { get; set; }
        [DataMember]
        public long? RequiredPlatformId { get; set; }
        [DataMember]
        public bool IsBlockedByRelease { get; set; }
        [DataMember]
        public int OrderWorkflowStepId { get; set; }
        [DataMember]
        public IEnumerable<AdvertisementDescriptor> Advertisements { get; set; }
        [DataMember]
        public bool IsRated { get; set; }
    }
}