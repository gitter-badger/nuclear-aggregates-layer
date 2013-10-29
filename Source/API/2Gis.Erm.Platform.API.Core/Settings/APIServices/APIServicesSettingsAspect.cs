using System;
using System.Collections.Generic;
using System.Configuration;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public sealed class APIServicesSettingsAspect
    {
        private readonly IReadOnlyDictionary<Type, IAPIServiceSettings> _availableServicesMap;

        public APIServicesSettingsAspect(IDictionary<Type, IAPIServiceSettings> globalServicesSettings)
        {
            var supportedServices = new List<IAPIServiceSettingsInitializer>
                {
                    new APIIdentityServiceSettings(),
                    new APIIntrospectionServiceSettings(),
                    new APIOperationsServiceSettings(),
                    new APIOrderValidationServiceSettings(),
                    new ApiMoDiServiceSettings(),
                    new ApiRealeasingServiceSettings(),
                    new APICostCalculationServiceSettings()
                };

            var availableServices = new Dictionary<Type, IAPIServiceSettings>(globalServicesSettings);
            foreach (var ermServiceConfig in ErmServiceDescriptionsConfiguration.ErmServices)
            {
                for (int i = 0; i < supportedServices.Count; i++)
                {
                    var currentProcessingService = supportedServices[i];
                    if (currentProcessingService.TryInitialize(ermServiceConfig))
                    {
                        supportedServices.RemoveAt(i);
                        availableServices[currentProcessingService.ConcreteSettingsInterface] = (IAPIServiceSettings)currentProcessingService;
                        break;
                    }
                }
            }

            _availableServicesMap = availableServices;
        }

        public IReadOnlyDictionary<Type, IAPIServiceSettings> AvailableServices
        {
            get
            {
                return _availableServicesMap;
            }
        }

        public TConcreteSettings GetSettings<TConcreteSettings>()
            where TConcreteSettings : IAPIServiceSettings
        {
            TConcreteSettings settings;
            if (!TryGetSettings(out settings))
            {
                throw new ConfigurationErrorsException("Application *.config file doesn't contains settings for API service settings contract type " + typeof(TConcreteSettings));
            }

            return settings;
        }

        public bool TryGetSettings<TConcreteSettings>(out TConcreteSettings settings)
            where TConcreteSettings : IAPIServiceSettings
        {
            settings = default(TConcreteSettings);
            IAPIServiceSettings resolvedSettings;
            if (!_availableServicesMap.TryGetValue(typeof(TConcreteSettings), out resolvedSettings))
            {
                return false;
            }

            settings = (TConcreteSettings)resolvedSettings;
            return true;
        }
    }
}
