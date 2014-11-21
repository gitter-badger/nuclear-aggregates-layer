using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails
{
    public sealed class ValidateCreateAccountDetailRequest : Request
    {
        public long AccountId { get; set; }
    }

    public sealed class ValidateCreateAccountDetailResponse : Response
    {
        public bool Validated { get; set; }
    }
}