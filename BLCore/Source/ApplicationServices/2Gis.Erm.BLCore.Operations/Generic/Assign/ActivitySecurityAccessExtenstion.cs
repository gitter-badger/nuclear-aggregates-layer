using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public static class ActivitySecurityAccessExtenstion
    {
        public static bool HasActivityUpdateAccess(this ISecurityServiceEntityAccess entityAccessService, IUserContext userContext, EntityName entityName, long entityId, long ownerCode)
        {
            if (!userContext.Identity.SkipEntityAccessCheck)
            {
                var ownerCanBeChanged = entityAccessService.HasEntityAccess(EntityAccessTypes.Update,
                                                                             entityName,
                                                                             userContext.Identity.Code,
                                                                             entityId,
                                                                             ownerCode,
                                                                             null);
                if (!ownerCanBeChanged)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
