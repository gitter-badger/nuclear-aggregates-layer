using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public class Log4NetConfigurationOperationContextExtension : WcfKeyValueExtension<OperationContext>
    {
        private static readonly IDictionary<string, Guid> LoggerContextKeysMap = new Dictionary<string, Guid>
            {
                { LoggerContextKeys.Required.SessionId, new Guid("{F03705C9-F03E-4AAB-8962-231EF741CDA4}") },
                { LoggerContextKeys.Required.UserName, new Guid("{5F03CBC3-4B5F-4C87-BA69-460E0031C221}") },
                { LoggerContextKeys.Required.UserIP, new Guid("{EBBD80D4-5301-488D-8339-03597BCAE606}") },
                { LoggerContextKeys.Required.UserBrowser, new Guid("{7DDBC585-5D2C-4377-8F49-697784C29A52}") },
                { LoggerContextKeys.Required.SeanceCode, new Guid("{D2892542-4B80-4C89-9363-8916F2635BB8}") }
            };

        public static Log4NetConfigurationOperationContextExtension Current
        {
            get { return OperationContext.Current.Extensions.Find<Log4NetConfigurationOperationContextExtension>(); }
        }

        public string GetLoggerContextValue(string loggerContextKey)
        {
            Guid contextKey;
            if (LoggerContextKeysMap.TryGetValue(loggerContextKey, out contextKey))
            {
                return (string)FindInstance(contextKey);
            }
            throw new ConfigurationErrorsException("loggerContextKey cannot be found. See Log4NetConfigurationOperationContextExtension for details");
        }

        public void SetLoggerContextValue(string loggerContextKey, string value)
        {
            Guid contextKey;
            if (LoggerContextKeysMap.TryGetValue(loggerContextKey, out contextKey))
            {
                RegisterInstance(contextKey, value);
            }
            else
            {
                throw new ConfigurationErrorsException("loggerContextKey cannot be found. See Log4NetConfigurationOperationContextExtension for details");
            }
        }
    }
}