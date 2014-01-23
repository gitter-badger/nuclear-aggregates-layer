namespace DoubleGis.Erm.Qds.API.Core.Settings
{
    public interface ISearchSettings
    {
        string Host { get; }
        Protocol Protocol { get; }
        int HttpPort { get; }
        int ThriftPort { get; }
        int BatchSize { get; }
    }
}