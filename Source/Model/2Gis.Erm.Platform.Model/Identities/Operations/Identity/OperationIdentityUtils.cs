using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public static class OperationIdentityUtils
    {
        public static IOperationIdentity ResolveTargetModelOperation<TBusinessModelIdentity, TSimplifiedModelIdentity>(this EntityName entityName)
            where TBusinessModelIdentity : OperationIdentityBase<TBusinessModelIdentity>, IBusinessModelIdentity, new()
            where TSimplifiedModelIdentity : OperationIdentityBase<TSimplifiedModelIdentity>, ISimplifiedModelIdentity, new()
        {
            return entityName.IsSimplifiedModel() 
                ? (IOperationIdentity)new TSimplifiedModelIdentity() 
                : (IOperationIdentity)new TBusinessModelIdentity();
        }

        public static bool IsEntitySpecific(this IOperationIdentity identity)
        {
            return identity is IEntitySpecificOperationIdentity;
        }

        public static bool IsNonCoupled(this IOperationIdentity identity)
        {
            return identity is INonCoupledOperationIdentity;
        }
    }
}
