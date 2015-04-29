using NuClear.Settings.API;

namespace NuClear.Storage.ConnectionStrings
{
    public interface IConnectionStringSettings : ISettings
    {
        string GetConnectionString(IConnectionStringIdentity connectionStringIdentity);
    }
}