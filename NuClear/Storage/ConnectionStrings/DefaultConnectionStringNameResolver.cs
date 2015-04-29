namespace NuClear.Storage.ConnectionStrings
{
    public class DefaultConnectionStringNameResolver : IConnectionStringNameResolver
    {
        private readonly IConnectionStringIdentity _defaultConnectionStringName;

        public DefaultConnectionStringNameResolver(IConnectionStringIdentity defaultConnectionStringName)
        {
            _defaultConnectionStringName = defaultConnectionStringName;
        }

        public IConnectionStringIdentity ResolveRead(string entityContainerName)
        {
            return _defaultConnectionStringName;
        }

        public IConnectionStringIdentity ResolveWrite(string entityContainerName)
        {
            return _defaultConnectionStringName;
        }
    }
}