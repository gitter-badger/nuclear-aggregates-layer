using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services
{
    public interface IUIServicesManager
    {
        IEntityGridViewService GetEntityGridViewService(EntityName entityName);
    }
}