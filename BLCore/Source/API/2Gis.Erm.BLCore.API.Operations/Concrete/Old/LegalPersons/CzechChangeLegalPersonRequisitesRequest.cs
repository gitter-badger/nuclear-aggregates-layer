using DoubleGis.Erm.Common.Localization;
using DoubleGis.Erm.Core.RequestResponse.Base;
using DoubleGis.Erm.Model.Entities.Enums;

namespace DoubleGis.Erm.Core.RequestResponse.LegalPersons
{
    public sealed class CzechChangeLegalPersonRequisitesRequest : Request, ICzechAdapted
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
        public string Ic { get; set; }
    }
}
