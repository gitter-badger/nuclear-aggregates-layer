using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ChargesHistoryDomainEntityDto : IDomainEntityDto<ChargesHistory>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ProjectRef { get; set; }

        [DataMember]
        public DateTime PeriodStartDate { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public EntityReference SessionRef { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }
    }
}