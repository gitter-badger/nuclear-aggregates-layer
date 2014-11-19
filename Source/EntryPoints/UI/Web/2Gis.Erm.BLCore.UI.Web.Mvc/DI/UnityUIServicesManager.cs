using System.Linq;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Microsoft.Practices.Unity;

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

        public IEntityGridViewService GetEntityGridViewService(EntityName entityName)
        {
            var gridViewServiceType = typeof(IGenericEntityGridViewService<>).MakeGenericType(entityName.AsEntityType());
            return (IEntityGridViewService)_container.Resolve(gridViewServiceType);
        }

        public IViewModelCustomizationService<TModel> GetModelCustomizationService<TModel, TEntity>() where TModel : class, IEntityViewModelBase where TEntity : class, IEntityKey
        {
            var customizationsTypes = _viewModelCustomizationProvider.GetCustomizations(typeof(TEntity).AsEntityName());
            var customizations = customizationsTypes.Select(type => (IViewModelCustomization<TModel>)_container.Resolve(type)).ToArray();
            return new GenericViewModelCustomizationService<TModel>(customizations);
        }
    }
}