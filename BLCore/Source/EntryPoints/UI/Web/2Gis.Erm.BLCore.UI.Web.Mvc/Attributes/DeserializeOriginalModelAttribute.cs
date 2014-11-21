using System;
using System.Linq;
using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class GetEntityStateTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionParameters = filterContext.ActionParameters;

            if (actionParameters == null || actionParameters.Count == 0)
            {
                return;
            }

            var viewModel = actionParameters.First().Value as IEntityViewModelBase;
            if (viewModel == null)
            {
                return;
            }

            viewModel.GetEntityStateToken();
        }
    }
}
