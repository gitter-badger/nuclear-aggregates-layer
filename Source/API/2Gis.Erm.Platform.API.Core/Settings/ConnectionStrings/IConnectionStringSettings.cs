using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public interface IConnectionStringSettings : ISettings
    {
        string GetConnectionString(ConnectionStringName connectionStringNameAlias);
        string ResolveConnectionStringName(ConnectionStringName connectionStringNameAlias);
        IReadOnlyDictionary<ConnectionStringName, string> AllConnections { get; }
    }
}