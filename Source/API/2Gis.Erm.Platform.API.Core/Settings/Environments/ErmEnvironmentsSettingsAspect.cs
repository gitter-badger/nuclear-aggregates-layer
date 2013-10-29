using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Environments
{
    public class ErmEnvironmentsSettingsAspect
    {
        private readonly IDictionary<ConnectionStringName, string> _connectionStringsMap = new Dictionary<ConnectionStringName, string>();
        private readonly IDictionary<Type, IAPIServiceSettings> _availableServicesMap = new Dictionary<Type, IAPIServiceSettings>();

        public ErmEnvironmentsSettingsAspect(string environmentName, string entryPointName)
        {
            var targetEnvironment = ErmEnvironmentDescriptionsConfiguration.Instance.Value.ErmEnvironments.Cast<ErmEnvironmentElement>()
                                                                           .SingleOrDefault(x => x.Name == environmentName);

            if (targetEnvironment == null)
            {
                return;
            }

            var entryPointOverride = targetEnvironment.EntryPointsOverrides.Cast<EntryPointOverrideElement>().SingleOrDefault(x => x.EntryPointName == entryPointName);

            InitServicesSettings(targetEnvironment.ErmServices.Cast<ErmServiceElement>(),
                                 entryPointOverride != null
                                     ? entryPointOverride.ErmServices.Cast<ErmServiceElement>()
                                     : new ErmServiceElement[0]);

            InitConnectionStrings(targetEnvironment.ConnectionStrings.Cast<ConnectionStringSettings>(),
                                  entryPointOverride != null
                                      ? entryPointOverride.ConnectionStrings.Cast<ConnectionStringSettings>()
                                      : new ConnectionStringSettings[0]);
        }

        public IDictionary<ConnectionStringName, string> ConnectionStrings
        {
            get { return _connectionStringsMap; }
        }

        public IDictionary<Type, IAPIServiceSettings> AvailableServices
        {
            get { return _availableServicesMap; }
        }

        private void InitConnectionStrings(IEnumerable<ConnectionStringSettings> connectionStrings, IEnumerable<ConnectionStringSettings> connectionStringSettingsOverrides)
        {
            var specifiedConnectionStringsMap = connectionStrings.ToDictionary(c => c.Name);
            foreach (var overridenStr in connectionStringSettingsOverrides)
            {
                specifiedConnectionStringsMap[overridenStr.Name] = overridenStr;
            }

            foreach (var connectionStringAlias in Enum.GetValues(typeof(ConnectionStringName)).Cast<ConnectionStringName>().Where(x => x != ConnectionStringName.None))
            {
                var connectionStringName = connectionStringAlias.ToDefaultConnectionStringName();
                ConnectionStringSettings connection;

                if (specifiedConnectionStringsMap.TryGetValue(connectionStringName, out connection))
                {
                    ConnectionStrings.Add(connectionStringAlias, connection.ConnectionString);
                }
            }
        }

        private void InitServicesSettings(IEnumerable<ErmServiceElement> servicesSettings, IEnumerable<ErmServiceElement> ermServiceElementOverrides)
        {
            var specifiedServices = servicesSettings.ToDictionary(x => x.ServiceName);
            foreach (var overridenService in ermServiceElementOverrides)
            {
                specifiedServices[overridenService.ServiceName] = overridenService;
            }

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

            foreach (var service in supportedServices)
            {
                foreach (var serviceConfig in specifiedServices.Values)
                {
                    if (service.TryInitialize(new ErmServiceDescriptionsConfiguration.ErmServiceDescription(serviceConfig.ServiceName,
                                                                                                            serviceConfig.RestUrl,
                                                                                                            serviceConfig.BaseUrl,
                                                                                                            serviceConfig.SoapEndpointName)))
                    {
                        AvailableServices.Add(service.ConcreteSettingsInterface, (IAPIServiceSettings)service);
                        break;
                    }
                }
            }
        }
    }
}