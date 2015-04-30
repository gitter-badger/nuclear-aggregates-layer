using System;

using NuClear.Storage.Core;
using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class PersistenceChangesRegistryKey : IContextKey<IPersistenceChangesRegistry>
    {
        private static readonly Lazy<PersistenceChangesRegistryKey> KeyInstance = new Lazy<PersistenceChangesRegistryKey>();

        public static PersistenceChangesRegistryKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}