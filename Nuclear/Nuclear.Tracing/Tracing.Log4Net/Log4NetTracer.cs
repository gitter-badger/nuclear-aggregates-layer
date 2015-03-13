using System.Globalization;

using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed partial class Log4NetTracer : ITracer
    {
        private readonly CultureInfo _tracingCulture;
        private readonly ILog _log;

        public Log4NetTracer(string tracerName, CultureInfo tracingCulture)
        {
            _tracingCulture = tracingCulture;
            _log = LogManager.GetLogger(tracerName);
        }
    }
}
