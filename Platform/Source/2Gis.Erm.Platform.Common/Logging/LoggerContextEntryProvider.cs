using System;

using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.Platform.Common.Logging
{
    public abstract class LoggerContextEntryProvider : ILoggerContextEntryProvider
    {
        private readonly string _loggerContextKey;

        protected LoggerContextEntryProvider(String loggerContextKey)
        {
            _loggerContextKey = loggerContextKey;
        }

        public string Key
        {
            get { return _loggerContextKey; }
        }

        public abstract string Value { get; set; }

        public sealed override string ToString()
        {
            return Value ?? ResPlatform.ValueNotFound;
        }
    }
}
