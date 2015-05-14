using NuClear.Storage.Core;

namespace Storage.Tests.Fakes
{
    public class StubReadDomainContextProvider : IReadDomainContextProvider
    {
        public IReadDomainContext Get<TEntity>() where TEntity : class
        {
            return new StubDomainContext();
        }

        public IReadDomainContext Get()
        {
            return new StubDomainContext();
        }
    }
}