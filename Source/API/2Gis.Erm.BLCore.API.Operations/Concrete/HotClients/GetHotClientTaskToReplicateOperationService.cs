using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients
{
    public interface IGetHotClientTaskToReplicateOperationService : IOperation<GetHotClientTaskToReplicateIdentity>
    {
        HotClientTaskDto GetHotClientTask(long hotClientRequestId);
    }
}