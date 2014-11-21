using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
// ReSharper restore CheckNamespace
{
    public sealed class EditAccountRequest : EditRequest<Account>
    {
        public bool IgnoreSecurity { get; set; }
    }

    public sealed class EditAccountResponse : Response
    {
        public long AccountId { get; set; }
    }
}