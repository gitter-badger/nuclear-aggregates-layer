using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.Platform.Model.Entities;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    // ReSharper disable InconsistentNaming
    public class UnityUIServicesManager : IUIServicesManager
    // ReSharper restore InconsistentNaming
    {
        private readonly IUnityContainer _container;

        public UnityUIServicesManager(IUnityContainer container)
        {
            _container = container;
        }

        public IEntityGridViewService GetEntityGridViewService(EntityName entityName)
        {
            var gridViewServiceType = typeof(IGenericEntityGridViewService<>).MakeGenericType(entityName.AsEntityType());
            return (IEntityGridViewService)_container.Resolve(gridViewServiceType);
        }
    }
}