using System.Diagnostics.CodeAnalysis;

using NuClear.Storage.Core;

namespace Aggregates.Storage.DI.Unity
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class ReadDomainContextProviderProxy : IReadDomainContextProvider
    {
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly IDomainContextHost _domainContextHost;

        public ReadDomainContextProviderProxy(ScopedDomainContextsStore scopedDomainContextsStore, IDomainContextHost domainContextHost)
        {
            _scopedDomainContextsStore = scopedDomainContextsStore;
            _domainContextHost = domainContextHost;
        }

        public IReadDomainContext Get()
        {
            return _scopedDomainContextsStore.GetReadable(_domainContextHost);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class ModifiableDomainContextProviderProxy : IModifiableDomainContextProvider
    {
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly IDomainContextHost _domainContextHost;

        public ModifiableDomainContextProviderProxy(ScopedDomainContextsStore scopedDomainContextsStore, IDomainContextHost domainContextHost)
        {
            _scopedDomainContextsStore = scopedDomainContextsStore;
            _domainContextHost = domainContextHost;
        }

        public IModifiableDomainContext Get<TEntity>() where TEntity : class
        {
            return _scopedDomainContextsStore.GetModifiable<TEntity>(_domainContextHost);
        }
    }
}
