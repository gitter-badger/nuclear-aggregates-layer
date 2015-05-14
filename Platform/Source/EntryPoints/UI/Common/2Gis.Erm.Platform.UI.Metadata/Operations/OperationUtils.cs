using System;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.UI.Metadata.Operations
{
    public static class OperationUtils
    {
        private static readonly Type ClientOperationIndicator = typeof(IClientOperationIdentity);

        public static bool IsClientOperation(this IOperationIdentity operationIdentity)
        {
            return operationIdentity is IClientOperationIdentity;
        }

        public static bool IsClientOperation<TOperationIdentity>()
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        {
            return typeof(TOperationIdentity).IsClientOperation();
        }

        public static bool IsClientOperation(this Type checkingType)
        {
            return ClientOperationIndicator.IsAssignableFrom(checkingType);
        }
    }
}
