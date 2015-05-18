using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class CardRelationDomainEntityDto : IDomainEntityDto<CardRelation>
    {
        [DataMember]
        public long DepCardCode { get; set; }

        [DataMember]
        public long? PosCardCode { get; set; }

        [DataMember]
        public int OrderNo { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public long Id { get; set; }
    }
}