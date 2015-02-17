﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.App_Start;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.DI;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Validators;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config;
using DoubleGis.Erm.Platform.Common.Logging.SystemInfo;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
using DoubleGis.Erm.UI.Web.Mvc.DI;
using DoubleGis.Erm.UI.Web.Mvc.Settings;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.UI.Web.Mvc
{
    public class MvcApplication : HttpApplication
    {
        private static IUnityContainer _container;
        private static bool _databaseSynchronized;
        private static ILoggerContextManager _loggerContextManager;

        private static ICommonLog Logger
        {
            get { return _container.Resolve<ICommonLog>(); }
        }

        private static ISignInService SignInService
        {
            get { return _container.Resolve<ISignInService>(); }
        }

        private static IUserProfileService UserProfileService
        {
            get { return _container.Resolve<IUserProfileService>(); }
        }

        private static IUserContextModifyAccessor UserContextModifyAccessor
        {
            get { return (IUserContextModifyAccessor)_container.Resolve<IUserContext>(); }
        }

        private static IDatabaseSyncChecker DatabaseSyncChecker
        {
            get { return _container.Resolve<IDatabaseSyncChecker>(); }
        }

        private static IDefaultUserContextConfigurator DefaultUserContextConfigurator
        {
            get { return _container.Resolve<IDefaultUserContextConfigurator>(); }
        }

        protected void Application_Start()
        {
            var settingsContainer = new WebAppSettings(BusinessModels.Supported);
            var environmentSettings = settingsContainer.AsSettings<IEnvironmentSettings>();

            var loggerContextEntryProviders =
                    new ILoggerContextEntryProvider[] 
                    {
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.Environment, environmentSettings.EnvironmentName),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPoint, environmentSettings.EntryPointName),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new LoggerContextEntryWebProvider(LoggerContextKeys.Required.UserAccount),
                        new LoggerContextEntryWebProvider(LoggerContextKeys.Optional.UserSession),
                        new LoggerContextEntryWebProvider(LoggerContextKeys.Optional.UserAddress),
                        new LoggerContextEntryWebProvider(LoggerContextKeys.Optional.UserAgent)
                    };

            _loggerContextManager = new LoggerContextManager(loggerContextEntryProviders);
            var logger = Log4NetLoggerBuilder.Use
                                             .DefaultXmlConfig
                                             .EventLog
                                             .DB(settingsContainer.AsSettings<IConnectionStringSettings>().LoggingConnectionString())
                                             .Build;

            // initialize unity
            _container = Bootstrapper.ConfigureUnity(settingsContainer, logger, _loggerContextManager);

            // set global dependency resolver
            DependencyResolver.SetResolver(_container.Resolve<UnityDependencyResolver>());

            // register asp.net mvc stuff
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.RegisterGlobalFilters(_container);
            RouteTable.Routes.Configure();

            // customizing validators (set all "required" attributes explicitly, register additional validators)
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(GreaterOrEqualThanAttribute), typeof(GreaterOrEqualThanValidator));

            _databaseSynchronized = !DatabaseSyncChecker.HasPendingMigrations();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (_databaseSynchronized)
            {
                return;
            }

            DatabaseSyncChecker.RefreshAppliedVersions();

            _databaseSynchronized = !DatabaseSyncChecker.HasPendingMigrations();
            if (_databaseSynchronized)
            {
                DatabaseSyncChecker.Close();
            }
            else
            {
                ExecuteErrorController("IncompatibleDBVersion");
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (Request.RawUrl.StartsWith("/__browserLink", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            LoggerContextPrepareForRequestProcessing();
            Logger.DebugFormat("Старт обработки запроса [{0}], queryString=[{1}]", Request.Path, Request.QueryString);

            // аутентифицируем и логиним пользователя
            var userInfo = SignInService.SignIn();

            // подключаем профиль пользователя
            var userProfile = UserProfileService.GetUserProfile(userInfo.Code);
            UserContextModifyAccessor.Profile = userProfile;

            // set to thread
            var currentThread = Thread.CurrentThread;
            currentThread.CurrentUICulture = userProfile.UserLocaleInfo.UserCultureInfo;
            currentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(userProfile.UserLocaleInfo.UserCultureInfo.Name);
        }

        protected void Application_ReleaseRequestState(object sender, EventArgs e)
        {
            Logger.DebugFormat("Окончание обработки запроса [{0}], queryString=[{1}]", Request.Path, Request.QueryString);
        }

        // error handling for non-500 errors
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
            {
                return;
            }

            var httpStatusCode = GetHttpStatusCodeForException(exception);
            switch (httpStatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    ExecuteErrorController("NonAuthenticated");
                    break;
                case HttpStatusCode.NotFound:
                    ExecuteErrorController("PageNotFound");
                    break;
                default:
                    Logger.Error(exception, "Unexpected error has occured");
                    break;
            }
        }

        // copy-paste from asp.net sources
        private static HttpStatusCode GetHttpStatusCodeForException(Exception exception)
        {
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                return (HttpStatusCode)httpException.GetHttpCode();
            }

            if (exception is UnauthorizedAccessException)
            {
                return HttpStatusCode.Unauthorized;
            }

            if (exception is PathTooLongException)
            {
                return HttpStatusCode.RequestUriTooLong;
            }

            var innerException = exception.InnerException;
            if (innerException != null)
            {
                return GetHttpStatusCodeForException(innerException);
            }

            return HttpStatusCode.InternalServerError;
        }

        private void ExecuteErrorController(string action)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                // do nothing
                return;
            }

            DefaultUserContextConfigurator.Configure(httpContext.Request);

            // do nothing if custom errors not enabled
            if (!httpContext.IsCustomErrorEnabled && _databaseSynchronized)
            {
                return;
            }

            var mvcHandler = httpContext.CurrentHandler as MvcHandler;

            // Охота получить контроллер и в отсутствие текущего хендлера. 
            // if (mvcHandler == null)
            // return;
            var requestContext = mvcHandler != null ? mvcHandler.RequestContext : Request.RequestContext;

            var controllerActivator = DependencyResolver.Current.GetService<IControllerActivator>();
            var errorController = controllerActivator.Create(requestContext, typeof(ErrorController));

            var routeValues = requestContext.RouteData.Values;
            routeValues["controller"] = "error";
            routeValues["action"] = action;

            var httpResponse = httpContext.Response;
            httpResponse.TrySkipIisCustomErrors = true;
            errorController.Execute(requestContext);
            Server.ClearError();
        }

        private void LoggerContextPrepareForRequestProcessing()
        {
            // log user data
            var userAccount = (User == null) ? "Не определено" : HttpContext.Current.User.Identity.Name;
            var userAddress = Request.UserHostAddress ?? "Не определено";
            var userAgent = (Request.Browser == null) ? "Не определено" : Request.Browser.Browser;

            _loggerContextManager.SetUserInfo(userAccount, Session.SessionID, userAddress, userAgent);
        }
    }
}
