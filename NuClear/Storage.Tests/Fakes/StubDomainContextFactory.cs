using NuClear.Storage.Core;

namespace Storage.Tests.Fakes
{
    public class StubDomainContextFactory : IReadDomainContextFactory, IModifiableDomainContextFactory
    {
        #region Implementation of IDomainContextFactory
        
        public IReadDomainContext Create(DomainContextMetadata contextMetadata)
        {
            return new StubDomainContext();
        }

        #endregion

        #region Implementation of IDomainContextFactory<out IModifiableDomainContext>

        IModifiableDomainContext IDomainContextFactory<IModifiableDomainContext>.Create<TEntity>()
        {
            return new StubDomainContext();
        }

        #endregion

    }
}
