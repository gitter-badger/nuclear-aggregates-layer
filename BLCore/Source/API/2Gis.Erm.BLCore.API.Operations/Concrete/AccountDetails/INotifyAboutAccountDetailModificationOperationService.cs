using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AccountDetails
{
    public interface INotifyAboutAccountDetailModificationOperationService : IOperation<NotifyAboutAccountDetailModificationIdentity>
    {
        void Notify(long accountDetailId, params long[] accountDetailIds);
    }
}
