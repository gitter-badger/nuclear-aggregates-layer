using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified
{
    public interface IHotClientRequestService : ISimplifiedModelConsumer
    {
        int CreateOrUpdate(HotClientRequest request);
        HotClientRequest GetHotClientRequest(long id);
        void LinkWithCrmTask(long id, Guid taskId);
    }
}
