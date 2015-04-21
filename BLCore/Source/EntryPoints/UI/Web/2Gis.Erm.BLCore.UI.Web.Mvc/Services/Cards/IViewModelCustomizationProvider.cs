using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationProvider
    {
        IEnumerable<IViewModelCustomization<TModel>> GetCustomizations<TModel, TEntity>() where TModel : IEntityViewModelBase where TEntity : IEntityKey;
    }
}