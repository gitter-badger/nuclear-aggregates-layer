using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Concrete.Old.LegalPersons
{
    public sealed class UkraineChangeLegalPersonRequisitesRequest : Request, IUkraineAdapted
    {
        public long LegalPersonId { get; set; }
        public string LegalName { get; set; }
        public string ShortName { get; set; }
        public LegalPersonType LegalPersonType { get; set; }
        public string LegalAddress { get; set; }
        public string Ipn { get; set; }
        public string Egrpou { get; set; }
        public TaxationType TaxationType { get; set; }
    }
}
