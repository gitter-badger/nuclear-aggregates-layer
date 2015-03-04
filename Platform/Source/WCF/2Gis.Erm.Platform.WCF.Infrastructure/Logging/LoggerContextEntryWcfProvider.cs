using System.ServiceModel;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public class LoggerContextEntryWcfProvider : LoggerContextEntryProvider
    {
        public LoggerContextEntryWcfProvider(string loggerContextKey) : base(loggerContextKey)
        {
        }

        public override string Value
        {
            get
            {
                if (OperationContext.Current == null)
                {
                    return null;
                }

                var configExtension = OperationContext.Current.Extensions.Find<Log4NetConfigurationOperationContextExtension>();
                return configExtension != null ? configExtension.GetLoggerContextValue(Key) : null;
            }

            set
            {
                if (OperationContext.Current == null)
                {
                    return;
                }

                var configExtension = OperationContext.Current.Extensions.Find<Log4NetConfigurationOperationContextExtension>();
                if (configExtension != null)
                {
                    configExtension.SetLoggerContextValue(Key, value);
                }
            }
        }
    }
}