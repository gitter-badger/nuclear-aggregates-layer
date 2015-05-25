using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
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

        public UnityUIServicesManager(IUnityContainer container)
        {
            _container = container;
        }

        public IEntityGridViewService GetEntityGridViewService(IEntityType entityName)
        {
            var gridViewServiceType = typeof(IGenericEntityGridViewService<>).MakeGenericType(entityName.AsEntityType());
            return (IEntityGridViewService)_container.Resolve(gridViewServiceType);
        }
    }
}