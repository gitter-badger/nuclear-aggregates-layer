using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    public sealed class UnityControllerActivator : IControllerActivator
    {
        private readonly IUnityContainer _container;
        
        public UnityControllerActivator(IUnityContainer container)
        {
            _container = container;
        }

        IController IControllerActivator.Create(RequestContext requestContext, Type controllerType)
        {
            try
            {
                return (IController)_container.Resolve(controllerType);
            }
            catch (Exception exception)
            {
                // if we cannot create controller, return 404 page
                throw new HttpException((int)HttpStatusCode.NotFound,
                                        string.Format(CultureInfo.CurrentCulture, BLResources.UrlDoesNotExists, requestContext.HttpContext.Request.Path),
                                        exception);
            }
        }
    }
}