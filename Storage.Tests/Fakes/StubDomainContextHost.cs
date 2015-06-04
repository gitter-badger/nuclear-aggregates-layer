using System;

using NuClear.Storage.Core;

namespace Storage.Tests.Fakes
{
    public class StubDomainContextHost : IDomainContextHost
    {
        public StubDomainContextHost(Guid scopeId)
        {
            ScopeId = scopeId;
        }

        public Guid ScopeId { get; private set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}