using DoubleGis.Erm.Platform.Common.Settings;

using Nest;

namespace DoubleGis.Erm.Qds.API.Core.Settings
{
    public interface INestSettings : ISettings
    {
        IConnectionSettingsValues ConnectionSettings { get; }
        Protocol Protocol { get; }
        int BatchSize { get; }
        string BatchTimeout { get; }

        void RegisterType<T>(string docIndexName, string docTypeName = null);
        string GetIsolatedIndexName(string indexName);
    }
}