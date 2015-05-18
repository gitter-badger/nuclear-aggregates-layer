using DoubleGis.Erm.Platform.API.Security.EntityAccess;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceEntityAccess
    {
        bool IsSecureEntity(IEntityType entityName);

        bool HasEntityAccess(EntityAccessTypes operationType, IEntityType entityName, long userCode, long? entityId, long ownerCode, long? oldOwnerCode);
        EntityAccessTypes RestrictEntityAccess(IEntityType entityName, EntityAccessTypes operationType, long userCode, long? entityId, long ownerCode, long? oldOwnerCode);
    }
}