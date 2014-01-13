using System;
using System.Linq;
using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UseDependencyFieldsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model;
            var viewModel = model as IConfigurableViewModel;
            if (viewModel == null)
            {
                return;
            }

            var dependencyList = model.GetType().GetProperties()
                                      .Where(pi => pi.IsDefined(typeof(DependencyAttribute), false))
                                      .SelectMany(pi => ((DependencyAttribute[])pi.GetCustomAttributes(typeof(DependencyAttribute), false))
                                                            .Select(x => new
                                                                {
                                                                    x.DependencyType,
                                                                    ConditionalFieldName = pi.Name,
                                                                    x.DependentFieldName,
                                                                    x.DependencyExpression
                                                                }))
                                      .GroupBy(attr => attr.ConditionalFieldName)
                                      .Select(x => new
                                          {
                                              TargetFieldId = x.Key,
                                              DependentFields = x.Select(y => new
                                                  {
                                                      Id = y.DependentFieldName,
                                                      Type = y.DependencyType,
                                                      Expression = y.DependencyExpression
                                                  })
                                          });
            if (dependencyList.Any())
            {
                viewModel.ViewConfig.DependencyList = JsonConvert.SerializeObject(dependencyList);
            }
        }
    }
}