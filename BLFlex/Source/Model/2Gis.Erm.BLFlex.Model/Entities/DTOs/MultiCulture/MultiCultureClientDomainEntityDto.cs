﻿using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture
{
    [DataContract]
    public class MultiCultureClientDomainEntityDto : IDomainEntityDto<Platform.Model.Entities.Erm.Client>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted, IKazakhstanAdapted
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string MainPhoneNumber { get; set; }
        [DataMember]
        public string AdditionalPhoneNumber1 { get; set; }
        [DataMember]
        public string AdditionalPhoneNumber2 { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Website { get; set; }
        [DataMember]
        public EntityReference MainFirmRef { get; set; }
        [DataMember]
        public string MainAddress { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public EntityReference TerritoryRef { get; set; }
        [DataMember]
        public InformationSource InformationSource { get; set; }
        [DataMember]
        public DateTime LastQualifyTime { get; set; }
        [DataMember]
        public DateTime? LastDisqualifyTime { get; set; }
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
    }
}
