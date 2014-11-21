using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AfterSaleServiceActivityDomainEntityDto : IDomainEntityDto<AfterSaleServiceActivity>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference DealRef { get; set; }

        [DataMember]
        public byte AfterSaleServiceType { get; set; }

        [DataMember]
        public int AbsoluteMonthNumber { get; set; }
    }
}