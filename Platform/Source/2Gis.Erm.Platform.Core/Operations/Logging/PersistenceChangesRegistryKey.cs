using System;

using DoubleGis.Erm.Platform.DAL;

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