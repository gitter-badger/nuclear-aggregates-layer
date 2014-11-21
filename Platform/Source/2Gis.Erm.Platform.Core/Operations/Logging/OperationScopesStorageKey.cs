using System;

using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationScopesStorageKey : IContextKey<OperationScopesStorage>
    {
        private static readonly Lazy<OperationScopesStorageKey> KeyInstance = new Lazy<OperationScopesStorageKey>();

        public static OperationScopesStorageKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}
