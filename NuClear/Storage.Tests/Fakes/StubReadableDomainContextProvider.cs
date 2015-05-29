using NuClear.Storage.Core;

namespace Storage.Tests.Fakes
{
    public class StubReadableDomainContextProvider : IReadableDomainContextProvider
    {
        public IReadableDomainContext Get<TEntity>() where TEntity : class
        {
            return new StubDomainContext();
        }

        public IReadableDomainContext Get()
        {
            return new StubDomainContext();
        }
    }
}