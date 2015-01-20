using System;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

using DoubleGis.Erm.API.WCF.Metadata.DI;
using DoubleGis.Erm.API.WCF.Metadata.Settings;
using DoubleGis.Erm.Platform.DI.WCF;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceHost;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.Metadata
{
    public sealed class UnityServiceHostFactory : UnityServiceHostFactoryBase<MetadataServiceAppSettings>
    {
        public UnityServiceHostFactory()
            : base(new MetadataServiceAppSettings(BusinessModels.Supported), Bootstrapper.ConfigureUnity)
        {
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new CustomAuthorizationServiceHost(DIContainer.ResolveAll<IAuthorizationPolicy>().ToArray(),
                                                                  DIContainer.Resolve<IServiceBehavior>(),
                                                                  serviceType,
                                                                  baseAddresses);
        }
    }
}