using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.App_Start
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(this GlobalFilterCollection filters, IUnityContainer container)
        {
            var exceptionFilter = container.Resolve<ExceptionFilter>();
            filters.Add(exceptionFilter);
        }
    }
}