using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config.Xml;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public static class RequiredServices
    {
        public static RequiredServicesBuilder Is<TServiceSettingsAspect>()
                where TServiceSettingsAspect : APIServiceSettingsBase, ISettingsAspect, new()
        {
            return new RequiredServicesBuilder().Is<TServiceSettingsAspect>();
        }

        public sealed class RequiredServicesBuilder
        {
            private readonly ICollection<APIServiceSettingsBase> _requiredServices = new List<APIServiceSettingsBase>();

            public static implicit operator ISettingsAspect[](RequiredServicesBuilder builder)
            {
                return builder.Apply();
            }

            public RequiredServicesBuilder Is<TServiceSettingsAspect>()
                where TServiceSettingsAspect : APIServiceSettingsBase, ISettingsAspect, new()
            {
                _requiredServices.Add(new TServiceSettingsAspect());
                return this;
            }

            public ISettingsAspect[] Apply()
            {
                return AttachAvailableServices(ErmServiceDescriptionsAccessor.GetErmServiceDescriptions());
            }

            public ISettingsAspect[] Apply(Configuration configuration)
            {
                return AttachAvailableServices(ErmServiceDescriptionsAccessor.GetErmServiceDescriptions(configuration));
            }

            private ISettingsAspect[] AttachAvailableServices(ErmServiceDescription[] actuallyConfiguredServices)
            {
                var aspects = new List<ISettingsAspect>();
                var notConfiguredServices = new List<string>();

                foreach (var requiredService in _requiredServices)
                {
                    var serviceConfig =
                        actuallyConfiguredServices.SingleOrDefault(service => string.CompareOrdinal(service.ServiceName, requiredService.Name) == 0);
                    if (serviceConfig == null)
                    {
                        notConfiguredServices.Add(requiredService.Name);
                        continue;
                    }

                    requiredService.Initialize(serviceConfig);
                    aspects.Add(requiredService);
                }

                if (notConfiguredServices.Any())
                {
                    throw new InvalidOperationException("Can't get required ERM services configuration. Services: " + string.Join(";", notConfiguredServices) +
                                                        ". Check settings in config");
                }

                return aspects.ToArray();
            }
        }
    }
}
