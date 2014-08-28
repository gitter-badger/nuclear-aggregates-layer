using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IChangeOrderProfilesOperationService : IOperation<ChangeOrderProfilesIdentity>
    {
        void ChangeProfiles(long orderId, long profileId);
    }
}
