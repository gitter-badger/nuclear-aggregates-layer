using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons
{
    public sealed class MergeLegalPersonsRequest : Request
    {
        public long AppendedLegalPersonId { get; set; }
        public long MainLegalPersonId { get; set; }
    }
}
