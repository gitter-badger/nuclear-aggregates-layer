using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ReleasesWithdrawalsPositionDomainEntityDto : IDomainEntityDto<ReleasesWithdrawalsPosition>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ReleasesWithdrawalRef { get; set; }

        [DataMember]
        public EntityReference PositionRef { get; set; }

        [DataMember]
        public EntityReference PlatformRef { get; set; }

        [DataMember]
        public decimal AmountToWithdraw { get; set; }

        [DataMember]
        public decimal Vat { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }
    }
}