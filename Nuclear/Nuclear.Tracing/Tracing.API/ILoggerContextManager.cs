namespace DoubleGis.Erm.Platform.Common.Logging
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