using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using log4net;

using Nuclear.Tracing.API;

namespace Nuclear.Tracing.Log4Net
{
    public sealed class TracerContextManager : ITracerContextManager
    {
        private readonly IReadOnlyDictionary<string, ITracerContextEntryProvider> _providers;

        public TracerContextManager(IEnumerable<ITracerContextEntryProvider> tracerContextEntryProviders)
        {
            var providers = new Dictionary<string, ITracerContextEntryProvider>();
            var requiredContextEntryProvidersRegistry =
                new HashSet<string>(
                    typeof(TracerContextKeys.Required)
                        .GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Where(fi => fi.IsLiteral)
                        .Select(fi => fi.GetValue(null))
                        .Cast<string>());

            foreach (var provider in tracerContextEntryProviders)
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
                throw new ApplicationException(string.Format("Required tracer context entry providers \"{0}\" is not specified", string.Join(";", requiredContextEntryProvidersRegistry)));
            }

            _providers = providers;
        }

        public string this[string entryKey]
        {
            get
            {
                ITracerContextEntryProvider provider;
                if (!_providers.TryGetValue(entryKey, out provider))
                {
                    throw new InvalidOperationException(string.Format("Can't get value, specified tracer context entry key \"{0}\" is not supported", entryKey));
                }

                return provider.Value;
            }
            set
            {
                ITracerContextEntryProvider provider;
                if (!_providers.TryGetValue(entryKey, out provider))
                {
                    throw new InvalidOperationException(string.Format("Can't set value, specified logger context entry key \"{0}\" is not supported", entryKey));
                }

                provider.Value = value;
            }
        }
    }
}
