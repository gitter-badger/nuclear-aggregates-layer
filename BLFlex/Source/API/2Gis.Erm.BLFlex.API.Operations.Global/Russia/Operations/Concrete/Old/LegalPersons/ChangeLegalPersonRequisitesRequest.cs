using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons
{
    public sealed class ChangeLegalPersonRequisitesRequest : Request, IRussiaAdapted
    {
        public long LegalPersonId { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        public LegalPersonType LegalPersonType { get; set; }
        public string LegalAddress { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string RegistrationAddress { get; set; }
    }
}
