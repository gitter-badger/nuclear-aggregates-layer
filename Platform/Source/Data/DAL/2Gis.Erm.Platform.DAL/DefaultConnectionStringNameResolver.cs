using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL
{
    public class DefaultConnectionStringNameResolver : IConnectionStringNameResolver
    {
        private readonly ConnectionStringName _defaultConnectionStringName;

        public DefaultConnectionStringNameResolver(ConnectionStringName defaultConnectionStringName)
        {
            _defaultConnectionStringName = defaultConnectionStringName;
        }

        public ConnectionStringName ResolveRead(string entityContainerName)
        {
            return _defaultConnectionStringName;
        }

        public ConnectionStringName ResolveWrite(string entityContainerName)
        {
            return _defaultConnectionStringName;
        }
    }
}