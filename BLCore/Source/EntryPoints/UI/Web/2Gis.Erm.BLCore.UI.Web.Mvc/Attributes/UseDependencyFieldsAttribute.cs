using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

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

            var dependencyList = GetViewModelProperties(model.GetType(), null)
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

        private static IEnumerable<DependencyDto> GetViewModelProperties(Type type, string parentFieldName)
        {
            var result = new List<DependencyDto>();
            foreach (var property in type.GetProperties())
            {
                if (property.IsDefined(typeof(DependencyAttribute), false))
                {
                    var propertyName = property.Name;
                    var dtos = property.GetCustomAttributes(typeof(DependencyAttribute), false)
                                       .Cast<DependencyAttribute>()
                                       .Select(x => new DependencyDto
                                           {
                                               DependencyType = x.DependencyType,
                                               ConditionalFieldName = FullName(parentFieldName, propertyName),
                                               DependentFieldName = FullName(parentFieldName, x.DependentFieldName),
                                               DependencyExpression = x.DependencyExpression
                                           });
                    result.AddRange(dtos);
                }

                if (typeof(IViewModel).IsAssignableFrom(property.PropertyType))
                {
                    result.AddRange(GetViewModelProperties(property.PropertyType, FullName(parentFieldName, property.Name)));
                }
            }

            return result;
        }

        private static string FullName(string parentFieldName, string propertyName)
        {
            return string.IsNullOrEmpty(parentFieldName)
                       ? propertyName
                       : string.Join(HtmlHelper.IdAttributeDotReplacement, parentFieldName, propertyName);
        }

        private class DependencyDto
        {
            public DependencyType DependencyType { get; set; }
            public string ConditionalFieldName { get; set; }
            public string DependentFieldName { get; set; }
            public string DependencyExpression { get; set; }
        }
    }
}