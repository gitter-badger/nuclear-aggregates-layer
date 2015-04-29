using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class PersistenceChangesRegistryProvider : IPersistenceChangesRegistryProvider
    {
        private readonly IProcessingContext _processingContext;

        public PersistenceChangesRegistryProvider(IProcessingContext processingContext)
        {
            _processingContext = processingContext;
        }

        public IPersistenceChangesRegistry ChangesRegistry 
        {
            get
            {
                return 
                    _processingContext.GetValue(PersistenceChangesRegistryKey.Instance, false) 
                        ?? new NullPersistenceChangesRegistry();
            }
        }
    }
}