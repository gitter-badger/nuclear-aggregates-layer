using System.Diagnostics.CodeAnalysis;

using NuClear.Storage.Core;

namespace NuClear.Aggregates.Storage.DI.Unity
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ReadableDomainContextProviderProxy : IReadableDomainContextProvider
    {
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly IDomainContextHost _domainContextHost;

        public ReadableDomainContextProviderProxy(ScopedDomainContextsStore scopedDomainContextsStore, IDomainContextHost domainContextHost)
        {
            _scopedDomainContextsStore = scopedDomainContextsStore;
            _domainContextHost = domainContextHost;
        }

        public IReadableDomainContext Get()
        {
            return _scopedDomainContextsStore.GetReadable(_domainContextHost);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ModifiableDomainContextProviderProxy : IModifiableDomainContextProvider
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
