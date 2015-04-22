using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Security.Principal;

using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Security;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public sealed class UnityAuthorizationPolicy : IAuthorizationPolicy
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly IUnityContainer _container;

        public UnityAuthorizationPolicy(IUnityContainer container)
        {
            _container = container;
        }

        string IAuthorizationComponent.Id
        {
            get { return _id.ToString(); }
        }

        ClaimSet IAuthorizationPolicy.Issuer
        {
            get { return ClaimSet.Windows; }
        }

        private IUserProfileService UserProfileService
        {
            get { return _container.Resolve<IUserProfileService>(); }
        }

        bool IAuthorizationPolicy.Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            var windowsCallerIdentity = GetCallerIdentity(evaluationContext); 
            var signInService = _container.Resolve<ISignInByIdentityService>();

            // Если на endpoint-е включена безопасность, то хост будет требовать аутентификации вызова и windowsCallerIdentity не будет равна null,
            // но если безопастность выключена, то windowsCallerIdentity == null, и в качестве контеста безопасности будет использована identity, под которой был запущен рантайм (пул приложений на IIS)
            var identity = windowsCallerIdentity ?? WindowsIdentity.GetCurrent();
            var userInfo = signInService.SignIn(identity);

            var userProfile = UserProfileService.GetUserProfile(userInfo.Code);
            var modifyContext = (IUserContextModifyAccessor)_container.Resolve<IUserContext>();
            modifyContext.Profile = userProfile;
            
            // т.к. custom principal теперь фактически не используется ERM, а evaluationContext.Properties["Principal"] присваивается просто, 
            // т.к. wcf ожидает, что он будет обязательно присвоен из-за укзанных настроек при регистрации UnityAuthorizationPolicy (конкретно - Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;)
            evaluationContext.Properties["Principal"] = new WcfWindowsPrincipal(identity);
            
            return true;
        }

        private static WindowsIdentity GetCallerIdentity(EvaluationContext evaluationContext)
        {
            object obj;
            if (!evaluationContext.Properties.TryGetValue("Identities", out obj))
            {
                return null;
            }

            var identities = obj as IList<IIdentity>;
            if (identities == null || identities.Count <= 0)
            {
                return null;
            }

            // this policy is intended for Windows accounts only
            var windowsClient = identities[0] as WindowsIdentity;
            if (windowsClient == null)
            {
                throw new SecurityTokenValidationException("Only Windows accounts supported");
            }

            return windowsClient;
        }
    }
}
