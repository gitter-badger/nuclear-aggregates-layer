using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed class LoggerContextManager : ILoggerContextManager
    {
        private readonly IReadOnlyDictionary<string, ILoggerContextEntryProvider> _providers;

        public LoggerContextManager(IEnumerable<ILoggerContextEntryProvider> loggerContextEntryProviders)
        {
            var providers = new Dictionary<string, ILoggerContextEntryProvider>();
            var requiredContextEntryProvidersRegistry =
                new HashSet<string>(
                    typeof(LoggerContextKeys.Required)
                        .GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Where(fi => fi.IsLiteral)
                        .Select(fi => fi.GetValue(null))
                        .Cast<string>());

            foreach (var provider in loggerContextEntryProviders)
            {
                if (requiredContextEntryProvidersRegistry.Contains(provider.Key))
                {
                    requiredContextEntryProvidersRegistry.Remove(provider.Key);
                }

                providers.Add(provider.Key, provider);
                GlobalContext.Properties[provider.Key] = provider;
            }

            if (requiredContextEntryProvidersRegistry.Any())
            {
                throw new ApplicationException(string.Format("Required logger context entry providers \"{0}\" is not specified", string.Join(";", requiredContextEntryProvidersRegistry)));
            }

            _providers = providers;
        }

        public string this[string entryKey]
        {
            get
            {
                ILoggerContextEntryProvider provider;
                if (!_providers.TryGetValue(entryKey, out provider))
                {
                    throw new InvalidOperationException(string.Format("Can't get value, specified logger context entry key \"{0}\" is not supported", entryKey));
                }

                return provider.Value;
            }
            set
            {
                ILoggerContextEntryProvider provider;
                if (!_providers.TryGetValue(entryKey, out provider))
                {
                    throw new InvalidOperationException(string.Format("Can't set value, specified logger context entry key \"{0}\" is not supported", entryKey));
                }

                provider.Value = value;
            }
        }
    }
}
