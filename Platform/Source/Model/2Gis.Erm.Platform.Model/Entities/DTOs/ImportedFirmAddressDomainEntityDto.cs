using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ImportedFirmAddressDomainEntityDto : IDomainEntityDto<ImportedFirmAddress>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }
}