using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients
{
    public interface IProcessHotClientRequestOperationService : IOperation<ProcessHotClientRequestIdentity>
    {
        void CreateHotClientTask(HotClientRequestDto hotClientRequest, long ownerId, RegardingObject regardingObject);
    }
}