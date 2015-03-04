using System.Globalization;

using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer : ITracer
    {
        private readonly CultureInfo _loggingCulture;
        private readonly ILog _log;

        public Log4NetTracer(string loggerName, CultureInfo loggingCulture)
        {
            _loggingCulture = loggingCulture;
            _log = LogManager.GetLogger(loggerName);
        }
    }
}
