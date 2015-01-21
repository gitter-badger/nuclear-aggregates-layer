using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public static class OperationIdentityUtils
    {
        public static IOperationIdentity ResolveTargetModelOperation<TBusinessModelIdentity, TSimplifiedModelIdentity>(this IEntityType entityName)
            where TBusinessModelIdentity : OperationIdentityBase<TBusinessModelIdentity>, IBusinessModelIdentity, new()
            where TSimplifiedModelIdentity : OperationIdentityBase<TSimplifiedModelIdentity>, ISimplifiedModelIdentity, new()
        {
            return entityName.IsSimplifiedModel() 
                ? (IOperationIdentity)new TSimplifiedModelIdentity() 
                : (IOperationIdentity)new TBusinessModelIdentity();
        }
    }
}
