namespace DoubleGis.Erm.Platform.DAL
{
    public class ReadDomainContextProviderProxy : IReadDomainContextProvider
    {
        private readonly IReadDomainContextProviderForHost _readDomainContextProviderForHost;
        private readonly IDomainContextHost _domainContextHost;

        public ReadDomainContextProviderProxy(IReadDomainContextProviderForHost readDomainContextProviderForHost, IDomainContextHost domainContextHost)
        {
            _readDomainContextProviderForHost = readDomainContextProviderForHost;
            _domainContextHost = domainContextHost;
        }

        public IReadDomainContext Get()
        {
            return _readDomainContextProviderForHost.Get(_domainContextHost);
        }
    }

    public class ModifiableDomainContextProviderProxy : IModifiableDomainContextProvider
    {
        private readonly IModifiableDomainContextProviderForHost _modifiableDomainContextFactoryForHost;
        private readonly IDomainContextHost _domainContextHost;

        public ModifiableDomainContextProviderProxy(IModifiableDomainContextProviderForHost modifiableDomainContextFactoryForHost, IDomainContextHost domainContextHost)
        {
            _modifiableDomainContextFactoryForHost = modifiableDomainContextFactoryForHost;
            _domainContextHost = domainContextHost;
        }

        public IModifiableDomainContext Get<TEntity>() where TEntity : class
        {
            return _modifiableDomainContextFactoryForHost.Get<TEntity>(_domainContextHost);
        }
    }
}
