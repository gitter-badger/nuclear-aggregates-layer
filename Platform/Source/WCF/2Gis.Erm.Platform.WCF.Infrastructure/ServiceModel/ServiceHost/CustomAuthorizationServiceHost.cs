using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel.Description;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceHost
{
    public class CustomAuthorizationServiceHost : System.ServiceModel.ServiceHost
    {
        private readonly IEnumerable<IAuthorizationPolicy> _authorizationPolicies;
        private readonly IServiceBehavior _serviceBehavior;

        public CustomAuthorizationServiceHost(
            IEnumerable<IAuthorizationPolicy> authorizationPolicies, 
            IServiceBehavior serviceBehavior, 
            Type serviceType, 
            params Uri[] baseAddresses)
        : base(serviceType, baseAddresses)
        {
            _authorizationPolicies = authorizationPolicies;
            _serviceBehavior = serviceBehavior;
            AddAuthorizationPolicies();
        }

        protected override void OnOpening()
        {
            var behaviors = Description.Behaviors;
            
            if (behaviors.All(b => b.GetType() != _serviceBehavior.GetType()))
            {
                behaviors.Add(_serviceBehavior);
            }

            base.OnOpening();
        }

        private void AddAuthorizationPolicies()
        {
            if (!_authorizationPolicies.Any())
            {
                return;
            }

            // if found authorization policy in service locator, use it anyway
            Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            var newPolicies = new List<IAuthorizationPolicy>(_authorizationPolicies);

            var oldPolicies = Authorization.ExternalAuthorizationPolicies;
            if (oldPolicies != null)
            {
                newPolicies.AddRange(oldPolicies);
            }

            Authorization.ExternalAuthorizationPolicies = newPolicies.AsReadOnly();
        }
    }
} 
