using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public static class StrictOperationIdentityUtils
    {
        public static StrictOperationIdentity SpecificFor<TEntity>(this IEntitySpecificOperationIdentity operationIdentity)
            where TEntity : class, IEntity
        {
            return new StrictOperationIdentity(operationIdentity, new[] { typeof(TEntity) }.AsEntitySet());
        }

        public static StrictOperationIdentity SpecificFor<TEntity1, TEntity2>(this IEntitySpecificOperationIdentity operationIdentity)
            where TEntity1 : class, IEntity
            where TEntity2 : class, IEntity
        {
            return new StrictOperationIdentity(operationIdentity, new[] { typeof(TEntity1), typeof(TEntity2) }.AsEntitySet());
        }

        public static StrictOperationIdentity SpecificFor(this IEntitySpecificOperationIdentity operationIdentity, params EntityName[] entityNames)
        {
            return new StrictOperationIdentity(operationIdentity, entityNames.ToEntitySet());
        }

        public static StrictOperationIdentity NonCoupled(this INonCoupledOperationIdentity operationIdentity)
        {
            return new StrictOperationIdentity(operationIdentity, EntitySet.Create.NonCoupled);
        }
    }
}