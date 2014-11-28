using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IConnectionStringNameResolver
    {
        ConnectionStringName ResolveRead(string entityContainerName);
        ConnectionStringName ResolveWrite(string entityContainerName);
    }
}