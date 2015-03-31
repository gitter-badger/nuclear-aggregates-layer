using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public static class ActivitySecurityAccessExtenstion
    {
        public static bool HasActivityUpdateAccess<T>(this ISecurityServiceEntityAccess entityAccessService, IUserIdentity identity, long entityId, long ownerCode)
        {
            return identity.SkipEntityAccessCheck || entityAccessService.HasEntityAccess(EntityAccessTypes.Update, typeof(T).AsEntityName(), identity.Code, entityId, ownerCode, null);
        }
    }
}