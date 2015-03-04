namespace Nuclear.Tracing.API
{
    public sealed class TracerContextSelfHostedEntryProvider : TracerContextEntryProvider
    {
        private string _value = "NOT_SET";

        public TracerContextSelfHostedEntryProvider(string tracerContextKey) 
            : base(tracerContextKey)
        {
        }

        public override string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}