using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Concrete.Old.LegalPersons
{
    public sealed class ChileChangeLegalPersonRequisitesRequest : Request, IChileAdapted
    {
        public long LegalPersonId { get; set; }
        public string LegalName { get; set; }
        public LegalPersonType LegalPersonType { get; set; }
        public string LegalAddress { get; set; }
        public string Rut { get; set; }
        public long CommuneId { get; set; }
    }
}