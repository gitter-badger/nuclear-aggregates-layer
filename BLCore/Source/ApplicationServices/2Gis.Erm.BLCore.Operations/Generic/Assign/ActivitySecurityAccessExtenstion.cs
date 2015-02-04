﻿using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

    public static class ActivitySecurityAccessExtenstion
    {
        public static bool HasActivityUpdateAccess<T>(this ISecurityServiceEntityAccess entityAccessService, IUserIdentity identity, long entityId, long ownerCode)
        {
            if (!identity.SkipEntityAccessCheck)
            {
                return entityAccessService.HasEntityAccess(EntityAccessTypes.Update, typeof(T).AsEntityName(), identity.Code, entityId, ownerCode, null);
            }

            return true;
        }
    }
}
