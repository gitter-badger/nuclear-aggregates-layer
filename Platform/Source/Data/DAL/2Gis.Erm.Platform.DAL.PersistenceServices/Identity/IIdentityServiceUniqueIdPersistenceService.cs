using System;

using DoubleGis.Erm.Platform.Model;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Identity
{
    public interface IIdentityServiceUniqueIdPersistenceService
    {
        bool TryGetFirstIdleId(int installationId, out byte id);
        void ReserveId(byte id, Guid serviceInstanceId);
    }
}