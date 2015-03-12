using System.Collections.Generic;
using System.Configuration;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public interface IConnectionStringSettings : ISettings
    {
        string GetConnectionString(ConnectionStringName connectionStringNameAlias);
        ConnectionStringSettings GetConnectionStringSettings(ConnectionStringName connectionStringNameAlias);
        string ResolveConnectionStringName(ConnectionStringName connectionStringNameAlias);
        IReadOnlyDictionary<ConnectionStringName, ConnectionStringSettings> AllConnections { get; }
    }
}