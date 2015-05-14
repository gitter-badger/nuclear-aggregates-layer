using System.Collections.Generic;

using NuClear.Settings.API;

namespace NuClear.Storage.ConnectionStrings
{
    public interface IConnectionStringSettings : ISettings
    {
        string GetConnectionString(IConnectionStringIdentity connectionStringIdentity);
        IReadOnlyDictionary<IConnectionStringIdentity, string> AllConnectionStrings { get; }
    }
}