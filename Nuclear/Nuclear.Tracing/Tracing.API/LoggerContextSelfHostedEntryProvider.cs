namespace Nuclear.Tracing.API
{
    public sealed class LoggerContextSelfHostedEntryProvider : LoggerContextEntryProvider
    {
        private string _value = "NOT_SET";

        public LoggerContextSelfHostedEntryProvider(string loggerContextKey) 
            : base(loggerContextKey)
        {
        }

        public override string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}