using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services
{
    public interface IUIServicesManager
    {
        IEntityGridViewService GetEntityGridViewService(IEntityType entityName);
    }
}