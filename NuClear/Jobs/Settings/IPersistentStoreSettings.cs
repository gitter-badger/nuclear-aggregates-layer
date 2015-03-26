using System.Configuration;

using NuClear.Settings.API;

namespace NuClear.Jobs.Settings
{
    public interface IPersistentStoreSettings : ISettings
    {
        ConnectionStringSettings ConnectionStringSettings { get; }
    }
}