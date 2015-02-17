namespace DoubleGis.Erm.Platform.Common.Logging
{
    public sealed class LoggerContextConstEntryProvider : LoggerContextEntryProvider
    {
        private readonly string _loggerContextValue;

        public LoggerContextConstEntryProvider(string loggerContextKey, string loggerContextValue) 
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