using System.Globalization;

using log4net;

namespace DoubleGis.Erm.Platform.Common.Logging.Log4Net
{
    public sealed partial class Log4NetCommonLog : ICommonLog
    {
        private readonly CultureInfo _loggingCulture;
        private readonly ILog _log;

        public Log4NetCommonLog(string loggerName, CultureInfo loggingCulture)
        {
            _loggingCulture = loggingCulture;
            _log = LogManager.GetLogger(loggerName);
        }
    }
}
