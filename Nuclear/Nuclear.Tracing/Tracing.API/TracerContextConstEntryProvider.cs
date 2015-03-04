namespace Nuclear.Tracing.API
{
    public sealed class TracerContextConstEntryProvider : TracerContextEntryProvider
    {
        private readonly string _loggerContextValue;

        public TracerContextConstEntryProvider(string loggerContextKey, string loggerContextValue) 
            : base(loggerContextKey)
        {
            _loggerContextValue = loggerContextValue;
        }

        public override string Value
        {
            get { return _loggerContextValue; }
            set { }
        }
    }
}