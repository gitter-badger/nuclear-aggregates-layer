using System;

using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubDomainContextHost : IDomainContextHost
    {
        public StubDomainContextHost(Guid scopeId)
        {
            ScopeId = scopeId;
        }

        #region Implementation of IDomainContextHost

        public Guid ScopeId { get; private set; }

        #endregion
    }
}