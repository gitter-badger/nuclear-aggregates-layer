using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.LegalPersons
{
    public sealed class EmiratesChangeLegalPersonRequisitesRequest : Request, IEmiratesAdapted
    {
        public long LegalPersonId { get; set; }
        public string LegalName { get; set; }
        public string LegalAddress { get; set; }
        public string CommercialLicense { get; set; }
        public DateTime CommercialLicenseBeginDate { get; set; }
        public DateTime CommercialLicenseEndDate { get; set; }
    }
}
