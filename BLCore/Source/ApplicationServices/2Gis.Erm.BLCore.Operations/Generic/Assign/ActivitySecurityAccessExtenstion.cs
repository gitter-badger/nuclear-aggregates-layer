using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using NuClear.Model.Common.Entities;

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
