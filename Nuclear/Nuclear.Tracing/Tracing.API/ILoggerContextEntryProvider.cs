namespace DoubleGis.Erm.Platform.Common.Logging
{
    public interface ILoggerContextEntryProvider
    {
        string Key { get; }
        string Value { get; set; }
    }
}