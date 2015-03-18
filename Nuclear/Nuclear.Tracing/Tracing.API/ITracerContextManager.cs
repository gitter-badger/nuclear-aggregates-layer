namespace Nuclear.Tracing.API
{
    public interface ITracerContextManager
    {
        string this[string entryKey]
        {
            get;
            set;
        }
    }
}