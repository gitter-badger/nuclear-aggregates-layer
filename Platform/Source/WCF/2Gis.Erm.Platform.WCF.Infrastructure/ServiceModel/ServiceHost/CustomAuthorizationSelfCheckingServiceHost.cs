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

        public CustomAuthorizationSelfCheckingServiceHost(IEnumerable<IAuthorizationPolicy> authorizationPolicies,
                                                          IServiceBehavior serviceBehavior,
                                                          Type serviceType,
                                                          Uri[] baseAddresses,
                                                          IServiceInstanceCheckinService serviceInstanceCheckinService)
            : base(authorizationPolicies, serviceBehavior, serviceType, baseAddresses)
        {
            _serviceInstanceCheckinService = serviceInstanceCheckinService;
        }

        protected override void OnOpening()
        {
            _serviceInstanceCheckinService.Faulted += OnFaulted;
            _serviceInstanceCheckinService.Start();

            base.OnOpening();
        }

        protected override void OnClosing()
        {
            _serviceInstanceCheckinService.Faulted -= OnFaulted;
            _serviceInstanceCheckinService.Stop();

            base.OnClosing();
        }

        private void OnFaulted(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Close();
        }
    }
}