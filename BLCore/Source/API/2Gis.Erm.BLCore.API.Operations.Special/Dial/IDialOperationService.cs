using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Dial;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Dial
{
    public interface IDialOperationService : IOperation<DialIdentity>
    {
        void Dial(string phone);
    }
}
