namespace NuClear.Storage.ConnectionStrings
{
    public interface IConnectionStringNameResolver
    {
        IConnectionStringIdentity ResolveRead(string entityContainerName);
        IConnectionStringIdentity ResolveWrite(string entityContainerName);
    }
}