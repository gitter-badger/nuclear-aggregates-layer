using DoubleGis.Erm.Platform.Common.Settings;

using Nest;

namespace DoubleGis.Erm.Qds.Common.Settings
{
    public interface INestSettings : ISettings
    {
        Protocol Protocol { get; }
        string IndexPrefix { get; }
        int BatchSize { get; }
        string BatchTimeout { get; }
        IConnectionSettingsValues ConnectionSettings { get; }
    }
}