using System.Collections.Generic;

namespace NuClear.Storage.ConnectionStrings
{
    public class ConnectionStringIdentityResolver : IConnectionStringIdentityResolver
    {
        private readonly IReadOnlyDictionary<string, IConnectionStringIdentity> _readConnectionStringNameMap;
        private readonly IReadOnlyDictionary<string, IConnectionStringIdentity> _writeConnectionStringNameMap;

        public ConnectionStringIdentityResolver(IReadOnlyDictionary<string, IConnectionStringIdentity> readConnectionStringNameMap,
                                                IReadOnlyDictionary<string, IConnectionStringIdentity> writeConnectionStringNameMap)
        {
            _readConnectionStringNameMap = readConnectionStringNameMap;
            _writeConnectionStringNameMap = writeConnectionStringNameMap;
        }

        public IConnectionStringIdentity ResolveRead(string entityContainerName)
        {
            return _readConnectionStringNameMap[entityContainerName];
        }

        public IConnectionStringIdentity ResolveWrite(string entityContainerName)
        {
            return _writeConnectionStringNameMap[entityContainerName];
        }
    }
}