using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils
{
    public class GenericContollerFactory : DefaultControllerFactory
    {
        private const string CreateOrUpdateControllerName = "CreateOrUpdate";
        private const string CrmCreateOrUpdateControllerName = "CrmCreateOrUpdate";
        private const string EntityTypeNameParameterName = "entityTypeName";

        private static readonly Lazy<IDictionary<Type, Type>> ViewModelTypes = new Lazy<IDictionary<Type, Type>>(() => new Dictionary<Type, Type>());
        private static readonly Lazy<IDictionary<Type, Type>> CreateOrUpdateContollerConcreteTypes = new Lazy<IDictionary<Type, Type>>(() => new Dictionary<Type, Type>());
        private static readonly Lazy<IDictionary<Type, Type>> CrmCreateOrUpdateContollerConcreteTypes = new Lazy<IDictionary<Type, Type>>(() => new Dictionary<Type, Type>());

        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IViewModelTypesRegistry _viewModelTypesRegistry;

        public GenericContollerFactory(IBusinessModelSettings businessModelSettings, IViewModelTypesRegistry viewModelTypesRegistry)
        {
            _businessModelSettings = businessModelSettings;
            _viewModelTypesRegistry = viewModelTypesRegistry;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            if (controllerName == CreateOrUpdateControllerName)
            {
                var entityTypeName = (string)requestContext.RouteData.Values[EntityTypeNameParameterName];
                IEntityType entityName;
                if (EntityType.Instance.TryParse(entityTypeName, out entityName))
                {
                    var entityType = entityName.AsEntityType();
                    var entityViewModelType = GetViewModelType(entityType);
                    if (entityViewModelType != null)
                    {
                        Type controllerType;
                        if (!CreateOrUpdateContollerConcreteTypes.Value.TryGetValue(entityType, out controllerType))
                        {
                            controllerType = typeof(CreateOrUpdateController<,>).MakeGenericType(new[] { entityType, entityViewModelType });
                            CreateOrUpdateContollerConcreteTypes.Value.Add(entityType, controllerType);
                        }

                        return controllerType;
                    }
                }
            }

            if (controllerName == CrmCreateOrUpdateControllerName)
            {
                var entityTypeName = (string)requestContext.RouteData.Values[EntityTypeNameParameterName];
                IEntityType entityName;
                if (EntityType.Instance.TryParse(entityTypeName, out entityName))
                {
                    var entityType = entityName.AsEntityType();
                    Type controllerType;
                    if (!CrmCreateOrUpdateContollerConcreteTypes.Value.TryGetValue(entityType, out controllerType))
                    {
                        controllerType = typeof(CrmCreateOrUpdateController<>).MakeGenericType(new[] { entityType });
                        CrmCreateOrUpdateContollerConcreteTypes.Value.Add(entityType, controllerType);
                    }

                    return controllerType;
                }
            }

            return base.GetControllerType(requestContext, controllerName);
        }

        private Type GetViewModelType(Type entityType)
        {
            Type viewModelType;
            if (!ViewModelTypes.Value.TryGetValue(entityType, out viewModelType))
            {
                var genericViewModelType = typeof(EntityViewModelBase<>).MakeGenericType(entityType);

                var viewModelTypes = _viewModelTypesRegistry.DeclaredViewModelTypes
                                                           .Where(x => genericViewModelType.IsAssignableFrom(x) &&
                                                                       x.Name.EndsWith(entityType.Name + "ViewModel"))
                                                           .ToArray();

                if (viewModelTypes.Any(x => typeof(IAdapted).IsAssignableFrom(x)))
                {
                    if (!viewModelTypes.All(x => typeof(IAdapted).IsAssignableFrom(x)))
                    {
                        throw new ConfigurationErrorsException("All adapted view models must be assignable from IAdapted interface");
                    }

                    viewModelType = viewModelTypes.SingleOrDefault(x => _businessModelSettings.BusinessModelIndicator.IsAssignableFrom(x));
                }
                else
                {
                    viewModelType = viewModelTypes.SingleOrDefault();
                }

                if (viewModelType != null)
                {
                    ViewModelTypes.Value.Add(entityType, viewModelType);
                }
            }

            return viewModelType;
        }
    }
}
