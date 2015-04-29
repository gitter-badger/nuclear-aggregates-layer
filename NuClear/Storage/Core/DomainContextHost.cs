using System;

namespace NuClear.Storage.Core
{
    public class DomainContextHost : IDomainContextHost
    {
        private readonly Guid _scopeId = Guid.NewGuid();

        public Guid ScopeId
        {
            get { return _scopeId; }
        }
    }
}