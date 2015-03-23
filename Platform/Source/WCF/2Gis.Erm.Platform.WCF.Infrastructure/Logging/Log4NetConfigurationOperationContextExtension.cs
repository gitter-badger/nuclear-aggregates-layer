using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;

using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public sealed class Log4NetConfigurationOperationContextExtension : WcfKeyValueExtension<OperationContext>
    {
        private static readonly IDictionary<string, Guid> TracerContextKeysMap = new Dictionary<string, Guid>
            {
                { TracerContextKeys.Required.Environment, new Guid("F03705C9-F03E-4AAB-8962-231EF741CDA4") },
                { TracerContextKeys.Required.EntryPoint, new Guid("5F03CBC3-4B5F-4C87-BA69-460E0031C221") },
                { TracerContextKeys.Required.EntryPointHost, new Guid("EBBD80D4-5301-488D-8339-03597BCAE606") },
                { TracerContextKeys.Required.EntryPointInstanceId, new Guid("7DDBC585-5D2C-4377-8F49-697784C29A52") },
                { TracerContextKeys.Required.UserAccount, new Guid("D2892542-4B80-4C89-9363-8916F2635BB8") },
                { TracerContextKeys.Optional.UserSession, new Guid("F9BF5404-67A2-4946-8873-CAF11377C8C0") },
                { TracerContextKeys.Optional.UserAddress, new Guid("92F9B236-B06A-4B64-9C4A-EDB8188C64F5") },
                { TracerContextKeys.Optional.UserAgent, new Guid("6520B987-7DBA-4A8D-875D-6A665DCC94D6") }
            };

        public static Log4NetConfigurationOperationContextExtension Current
        {
            get { return OperationContext.Current.Extensions.Find<Log4NetConfigurationOperationContextExtension>(); }
        }

        public string GetTracerContextValue(string tracerContextKey)
        {
            Guid contextKey;
            if (TracerContextKeysMap.TryGetValue(tracerContextKey, out contextKey))
            {
                return (string)FindInstance(contextKey);
            }
            throw new ConfigurationErrorsException("tracerContextKey cannot be found. See Log4NetConfigurationOperationContextExtension for details");
        }

        public void SetTracerContextValue(string tracerContextKey, string value)
        {
            Guid contextKey;
            if (TracerContextKeysMap.TryGetValue(tracerContextKey, out contextKey))
            {
                RegisterInstance(contextKey, value);
            }
            else
            {
                throw new ConfigurationErrorsException("tracerContextKey cannot be found. See Log4NetConfigurationOperationContextExtension for details");
            }
        }
    }
}