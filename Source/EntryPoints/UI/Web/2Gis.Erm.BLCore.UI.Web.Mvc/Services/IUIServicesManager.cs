using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Grid;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services
{
    public interface IUIServicesManager
    {
        IEntityGridViewService GetEntityGridViewService(EntityName entityName);
        IViewModelCustomizationService<TModel> GetModelCustomizationService<TModel, TEntity>()
            where TEntity : class, IEntityKey
            where TModel : class, IEntityViewModelBase;
    }
}