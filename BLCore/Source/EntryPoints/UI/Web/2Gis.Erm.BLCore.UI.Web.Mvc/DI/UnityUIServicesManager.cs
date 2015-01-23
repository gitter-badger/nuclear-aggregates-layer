using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    // ReSharper disable InconsistentNaming
    public class UnityUIServicesManager : IUIServicesManager
    // ReSharper restore InconsistentNaming
    {
        private readonly IUnityContainer _container;
        private readonly IViewModelCustomizationProvider _viewModelCustomizationProvider;

        public UnityUIServicesManager(IUnityContainer container, IViewModelCustomizationProvider viewModelCustomizationProvider)
        {
            _container = container;
            _viewModelCustomizationProvider = viewModelCustomizationProvider;
        }

        public IEntityGridViewService GetEntityGridViewService(IEntityType entityName)
        {
            var gridViewServiceType = typeof(IGenericEntityGridViewService<>).MakeGenericType(entityName.AsEntityType());
            return (IEntityGridViewService)_container.Resolve(gridViewServiceType);
        }

        public IViewModelCustomizationService GetModelCustomizationService(IEntityType entityName)
        {
            var viewModelCustomizationServiceType = typeof(IGenericViewModelCustomizationService<>).MakeGenericType(entityName.AsEntityType());

            var customizationsTypes = _viewModelCustomizationProvider.GetCustomizations(entityName);
            var customizations = customizationsTypes.Select(type => (IViewModelCustomization)_container.Resolve(type)).ToArray();

            object viewModelCustomizationService;
            if (customizations.Any())
            {
                viewModelCustomizationService = _container.Resolve(viewModelCustomizationServiceType,
                                                                   new DependencyOverride<IEnumerable<IViewModelCustomization>>(customizations));
            }
            else
            {
                viewModelCustomizationService = _container.Resolve(viewModelCustomizationServiceType);
            }

            return (IViewModelCustomizationService)viewModelCustomizationService;
        }
    }
}