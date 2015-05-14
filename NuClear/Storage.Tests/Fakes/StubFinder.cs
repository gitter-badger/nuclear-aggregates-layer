using NuClear.Storage;
using NuClear.Storage.Core;

namespace Storage.Tests.Fakes
{
    public class StubFinder : Finder
    {
        private readonly IReadDomainContextProvider _readDomainContextProvider;

        public StubFinder(IReadDomainContextProvider readDomainContextProvider)
            : base(readDomainContextProvider)
        {
            _readDomainContextProvider = readDomainContextProvider;
        }

        public IReadDomainContextProvider ReadDomainContextProvider
        {
            get
            {
                return _readDomainContextProvider;
            }
        }
    }
}
