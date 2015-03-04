namespace Nuclear.Tracing.API
{
    public interface ILoggerContextEntryProvider
    {
        string Key { get; }
        string Value { get; set; }
    }
}