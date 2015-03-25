using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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