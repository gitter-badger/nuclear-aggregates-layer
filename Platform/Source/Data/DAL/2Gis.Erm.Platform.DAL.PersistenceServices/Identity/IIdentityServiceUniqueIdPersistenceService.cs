using System;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Identity
{
    public interface IIdentityServiceUniqueIdPersistenceService
    {
        bool TryGetFirstIdleId(out byte id);
        void ReserveId(byte id, Guid serviceInstanceId);
        bool IsIdReservedBy(byte id, Guid serviceInstanceId);
    }
}