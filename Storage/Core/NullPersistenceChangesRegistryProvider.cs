namespace NuClear.Storage.Core
{
    public sealed class NullPersistenceChangesRegistryProvider : IPersistenceChangesRegistryProvider
    {
        public IPersistenceChangesRegistry ChangesRegistry 
        {
            get { return new NullPersistenceChangesRegistry(); }
        }
    }
}