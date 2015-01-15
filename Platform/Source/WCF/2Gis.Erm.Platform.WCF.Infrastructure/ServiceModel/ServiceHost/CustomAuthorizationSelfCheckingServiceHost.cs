using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.ServiceModel.Description;

using DoubleGis.Erm.Platform.API.Core.Checkin;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceHost
{
    public class CustomAuthorizationSelfCheckingServiceHost : CustomAuthorizationServiceHost
    {
        private readonly IServiceInstanceCheckinService _serviceInstanceCheckinService;
        private readonly IServiceInstanceIdProviderHolder _serviceInstanceIdProviderHolder;

        public CustomAuthorizationSelfCheckingServiceHost(IEnumerable<IAuthorizationPolicy> authorizationPolicies, IServiceBehavior serviceBehavior, Type serviceType, Uri[] baseAddresses, IServiceInstanceCheckinService serviceInstanceCheckinService, IServiceInstanceIdProviderHolder serviceInstanceIdProviderHolder)
            : base(authorizationPolicies, serviceBehavior, serviceType, baseAddresses)
        {
            _serviceInstanceCheckinService = serviceInstanceCheckinService;
            _serviceInstanceIdProviderHolder = serviceInstanceIdProviderHolder;
        }

        protected override void OnOpening()
        {
            _serviceInstanceCheckinService.Faulted += OnFaulted;
            _serviceInstanceCheckinService.Start();
            _serviceInstanceIdProviderHolder.SetProvider((IServiceInstanceIdProvider)_serviceInstanceCheckinService);

            base.OnOpening();
        }

        protected override void OnClosing()
        {
            _serviceInstanceCheckinService.Faulted -= OnFaulted;
            _serviceInstanceCheckinService.Stop();
            _serviceInstanceIdProviderHolder.RemoveProvider((IServiceInstanceIdProvider)_serviceInstanceCheckinService);

            base.OnClosing();
        }

        private void OnFaulted(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Close();
        }
    }
}