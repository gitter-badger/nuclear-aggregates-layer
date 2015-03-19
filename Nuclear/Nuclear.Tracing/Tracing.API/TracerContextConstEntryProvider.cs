namespace Nuclear.Tracing.API
{
    public sealed class TracerContextConstEntryProvider : TracerContextEntryProvider
    {
        private readonly string _tracerContextValue;

        public TracerContextConstEntryProvider(string tracerContextKey, string tracerContextValue) 
            : base(tracerContextKey)
        {
            _tracerContextValue = tracerContextValue;
        }

        public override string Value
        {
            get { return _tracerContextValue; }
            set { }
        }
    }
}