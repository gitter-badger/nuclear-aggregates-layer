﻿using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
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
