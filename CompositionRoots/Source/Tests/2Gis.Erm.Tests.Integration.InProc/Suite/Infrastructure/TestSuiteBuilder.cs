﻿using System;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Tests.Integration.InProc.DI;

using Microsoft.Practices.Unity;

using Nuclear.Settings.API;
using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public static class TestSuiteBuilder
    {
        public static bool TryBuildSuite(
            ISettingsContainer settingsContainer, 
            ITracer logger, 
            ITracerContextManager tracerContextManager,
            out ITestRunner testRunner)
        {
            testRunner = null;

            logger.InfoFormat("Building test suite {0} started", Assembly.GetExecutingAssembly().GetName().Name);
            
            try
            {
                var diContainer = Bootstrapper.ConfigureUnity(settingsContainer, logger, tracerContextManager);
                logger.Info("SignIn current user");
                SignIn(diContainer);
                logger.Info("Resolving tests runner");
                testRunner = diContainer.Resolve<ITestRunner>();
                logger.Info("Test suite build was successfully finished");
            }
            catch (Exception ex)
            {
                logger.FatalFormat(ex, "Can't build test suite");
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