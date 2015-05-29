using NuClear.Storage;
using NuClear.Storage.Core;

namespace Storage.Tests.Fakes
{
    public class StubFinder : Finder
    {
        private readonly IReadableDomainContextProvider _readableDomainContextProvider;

        public StubFinder(IReadableDomainContextProvider readableDomainContextProvider)
            : base(readableDomainContextProvider)
        {
            _readableDomainContextProvider = readableDomainContextProvider;
        }

        public IReadableDomainContextProvider ReadableDomainContextProvider
        {
            get
            {
                return _readableDomainContextProvider;
            }
        }
    }
}
