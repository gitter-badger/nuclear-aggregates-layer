namespace NuClear.Storage.ConnectionStrings
{
    public class DefaultConnectionStringIdentityResolver : IConnectionStringIdentityResolver
    {
        private readonly IConnectionStringIdentity _defaultConnectionStringName;

        public DefaultConnectionStringIdentityResolver(IConnectionStringIdentity defaultConnectionStringName)
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