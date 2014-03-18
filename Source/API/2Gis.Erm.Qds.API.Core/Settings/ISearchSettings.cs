using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Qds.API.Core.Settings
{
    public interface ISearchSettings : ISettings
    {
        string Host { get; }
        string IndexPrefix { get; }
        Protocol Protocol { get; }
        int HttpPort { get; }
        int ThriftPort { get; }
        int BatchSize { get; }
    }
}