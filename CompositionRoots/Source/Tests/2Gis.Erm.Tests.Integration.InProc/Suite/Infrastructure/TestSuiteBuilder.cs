using System;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Tests.Integration.InProc.DI;

using Microsoft.Practices.Unity;

using Nuclear.Settings.API;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public static class TestSuiteBuilder
    {
        public static bool TryBuildSuite(
            ISettingsContainer settingsContainer, 
            ITracer tracer, 
            ITracerContextManager tracerContextManager,
            out ITestRunner testRunner)
        {
            testRunner = null;

            tracer.InfoFormat("Building test suite {0} started", Assembly.GetExecutingAssembly().GetName().Name);
            
            try
            {
                var diContainer = Bootstrapper.ConfigureUnity(settingsContainer, tracer, tracerContextManager);
                tracer.Info("SignIn current user");
                SignIn(diContainer);
                tracer.Info("Resolving tests runner");
                testRunner = diContainer.Resolve<ITestRunner>();
                tracer.Info("Test suite build was successfully finished");
            }
            catch (Exception ex)
            {
                tracer.FatalFormat(ex, "Can't build test suite");
                return false;
            }

            return true;
        }

        private static void SignIn(IUnityContainer unityContainer)
        {
            var signInService = unityContainer.Resolve<ISignInService>();
            var userInfo = signInService.SignIn();
            var userProfileService = unityContainer.Resolve<IUserProfileService>();
            var userProfile = userProfileService.GetUserProfile(userInfo.Code);
            var modifyContext = (IUserContextModifyAccessor)unityContainer.Resolve<IUserContext>();
            modifyContext.Profile = userProfile;
        }
    }
}