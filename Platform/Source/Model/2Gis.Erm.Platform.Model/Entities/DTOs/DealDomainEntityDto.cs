using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class DealDomainEntityDto : IDomainEntityDto<Deal>
    {
    	[DataMember]
        public long Id { get; set; }

    	[DataMember]
        public Guid ReplicationCode { get; set; }

    	[DataMember]
        public string Name { get; set; }

    	[DataMember]
        public EntityReference MainFirmRef { get; set; }

    	[DataMember]
        public EntityReference ClientRef { get; set; }

    	[DataMember]
        public EntityReference CurrencyRef { get; set; }

    	[DataMember]
        public ReasonForNewDeal StartReason { get; set; }

    	[DataMember]
        public CloseDealReason CloseReason { get; set; }

    	[DataMember]
        public string CloseReasonOther { get; set; }

    	[DataMember]
        public DateTime? CloseDate { get; set; }

    	[DataMember]
        public string Comment { get; set; }

    	[DataMember]
        public bool IsDeleted { get; set; }

    	[DataMember]
        public bool IsActive { get; set; }

    	[DataMember]
        public EntityReference OwnerRef { get; set; }

    	[DataMember]
        public EntityReference CreatedByRef { get; set; }

    	[DataMember]
        public EntityReference ModifiedByRef { get; set; }

    	[DataMember]
        public DateTime CreatedOn { get; set; }

    	[DataMember]
        public DateTime? ModifiedOn { get; set; }

    	[DataMember]
        public byte[] Timestamp { get; set; }

    	[DataMember]
        public DealStage DealStage { get; set; }
    	[DataMember]
        public EntityReference BargainRef { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> AdvertisingCampaignBeginDate { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> AdvertisingCampaignEndDate { get; set; }
    	[DataMember]
        public string AdvertisingCampaignGoalText { get; set; }
    	[DataMember]
        public Nullable<AdvertisingCampaignGoals> AdvertisingCampaignGoals { get; set; }
    	[DataMember]
        public Nullable<PaymentFormat> PaymentFormat { get; set; }
    	[DataMember]
        public Nullable<decimal> AgencyFee { get; set; }

        [DataMember]
        public Guid ClientReplicationCode { get; set; }

        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }
    }
}
