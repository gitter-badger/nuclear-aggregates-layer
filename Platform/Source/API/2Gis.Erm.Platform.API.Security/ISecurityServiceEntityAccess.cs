using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceEntityAccess
    {
        bool IsSecureEntity(EntityName entityName);

        bool HasEntityAccess(EntityAccessTypes operationType, EntityName entityName, long userCode, long? entityId, long ownerCode, long? oldOwnerCode);
        EntityAccessTypes RestrictEntityAccess(EntityName entityName, EntityAccessTypes operationType, long userCode, long? entityId, long ownerCode, long? oldOwnerCode);
    }
}