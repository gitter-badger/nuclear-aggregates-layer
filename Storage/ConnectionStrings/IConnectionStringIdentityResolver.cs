namespace NuClear.Storage.ConnectionStrings
{
    public interface IConnectionStringIdentityResolver
    {
        IConnectionStringIdentity ResolveRead(string entityContainerName);
        IConnectionStringIdentity ResolveWrite(string entityContainerName);
    }
}