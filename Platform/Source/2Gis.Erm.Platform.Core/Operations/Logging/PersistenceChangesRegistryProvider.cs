using NuClear.Storage.Core;
using NuClear.Storage.UseCases;

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