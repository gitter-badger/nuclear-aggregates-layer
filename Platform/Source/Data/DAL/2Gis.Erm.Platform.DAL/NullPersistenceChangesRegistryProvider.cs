namespace DoubleGis.Erm.Platform.DAL
{
    public sealed class NullPersistenceChangesRegistryProvider : IPersistenceChangesRegistryProvider
    {
        public IPersistenceChangesRegistry ChangesRegistry 
        {
            get { return new NullPersistenceChangesRegistry(); }
        }
    }
}