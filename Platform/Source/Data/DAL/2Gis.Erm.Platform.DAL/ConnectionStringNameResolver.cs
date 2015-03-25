using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL
{
    public class ConnectionStringNameResolver : IConnectionStringNameResolver
    {
        private readonly IReadOnlyDictionary<string, ConnectionStringName> _readConnectionStringNameMap;
        private readonly IReadOnlyDictionary<string, ConnectionStringName> _writeConnectionStringNameMap;

        public ConnectionStringNameResolver(IReadOnlyDictionary<string, ConnectionStringName> readConnectionStringNameMap,
                                            IReadOnlyDictionary<string, ConnectionStringName> writeConnectionStringNameMap)
        {
            _readConnectionStringNameMap = readConnectionStringNameMap;
            _writeConnectionStringNameMap = writeConnectionStringNameMap;
        }

        public ConnectionStringName ResolveRead(string entityContainerName)
        {
            return _readConnectionStringNameMap[entityContainerName];
        }

        public ConnectionStringName ResolveWrite(string entityContainerName)
        {
            return _writeConnectionStringNameMap[entityContainerName];
        }
    }
}