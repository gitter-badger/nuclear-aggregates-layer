using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public sealed class Log4NetConfigurationOperationContextExtension : WcfKeyValueExtension<OperationContext>
    {
        private static readonly IDictionary<string, Guid> LoggerContextKeysMap = new Dictionary<string, Guid>
            {
                { LoggerContextKeys.Required.Environment, new Guid("F03705C9-F03E-4AAB-8962-231EF741CDA4") },
                { LoggerContextKeys.Required.EntryPoint, new Guid("5F03CBC3-4B5F-4C87-BA69-460E0031C221") },
                { LoggerContextKeys.Required.EntryPointHost, new Guid("EBBD80D4-5301-488D-8339-03597BCAE606") },
                { LoggerContextKeys.Required.EntryPointInstanceId, new Guid("7DDBC585-5D2C-4377-8F49-697784C29A52") },
                { LoggerContextKeys.Required.UserAccount, new Guid("D2892542-4B80-4C89-9363-8916F2635BB8") },
                { LoggerContextKeys.Optional.UserSession, new Guid("F9BF5404-67A2-4946-8873-CAF11377C8C0") },
                { LoggerContextKeys.Optional.UserAddress, new Guid("92F9B236-B06A-4B64-9C4A-EDB8188C64F5") },
                { LoggerContextKeys.Optional.UserAgent, new Guid("6520B987-7DBA-4A8D-875D-6A665DCC94D6") }
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