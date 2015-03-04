namespace Nuclear.Tracing.API
{
    public interface ILoggerContextManager
    {
        string this[string entryKey]
        {
            get;
            set;
        }
    }
}