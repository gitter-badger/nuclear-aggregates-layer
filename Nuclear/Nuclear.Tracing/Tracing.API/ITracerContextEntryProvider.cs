namespace Nuclear.Tracing.API
{
    public interface ITracerContextEntryProvider
    {
        string Key { get; }
        string Value { get; set; }
    }
}