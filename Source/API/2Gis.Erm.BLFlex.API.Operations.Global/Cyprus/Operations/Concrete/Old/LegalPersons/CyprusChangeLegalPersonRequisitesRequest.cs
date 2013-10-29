using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Core.RequestResponse.LegalPersons
{
    public sealed class CyprusChangeLegalPersonRequisitesRequest : Request, ICyprusAdapted
    {
        public long LegalPersonId { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        public LegalPersonType LegalPersonType { get; set; }
        public string CardNumber { get; set; }
        public string LegalAddress { get; set; }
        public string Inn { get; set; }
        public string PassportNumber { get; set; }
        public string RegistrationAddress { get; set; }
        public string VAT { get; set; }
    }
}
