using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    [DataContract]
    public class ChileLegalPersonDomainEntityDto : LegalPersonDomainEntityDto, IChileAdapted
    {
        [DataMember]
        public string OperationsKind { get; set; }
        [DataMember]
        public EntityReference CommuneRef { get; set; }
    }
}
