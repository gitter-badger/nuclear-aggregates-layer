using System.Collections.Generic;
using System.Configuration;

using DoubleGis.Erm.Platform.Common.Settings;

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