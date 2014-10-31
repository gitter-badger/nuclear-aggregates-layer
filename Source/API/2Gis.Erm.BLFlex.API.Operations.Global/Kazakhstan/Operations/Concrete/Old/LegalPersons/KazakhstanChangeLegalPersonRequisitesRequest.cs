using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Kazakhstan.Operations.Concrete.Old.LegalPersons
{
    public class KazakhstanChangeLegalPersonRequisitesRequest : Request
    {
        public long LegalPersonId { get; set; }
        public string Bin { get; set; }
        public string LegalAddress { get; set; }
        public string LegalName { get; set; }
        public LegalPersonType LegalPersonType { get; set; }
        public string IdentityCardNumber { get; set; }
        public string IdentityCardIssuedBy { get; set; }
        public DateTime? IdentityCardIssuedOn { get; set; }
    }
}