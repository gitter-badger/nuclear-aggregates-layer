using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture
{
    [DataContract]
    public class MultiCultureBargainTypeDomainEntityDto : IDomainEntityDto<Platform.Model.Entities.Erm.BargainType>, IChileAdapted, ICyprusAdapted,
                                                          ICzechAdapted, IUkraineAdapted, IKazakhstanAdapted
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public decimal VatRate { get; set; }

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
    }
}