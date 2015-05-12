using System;

using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
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