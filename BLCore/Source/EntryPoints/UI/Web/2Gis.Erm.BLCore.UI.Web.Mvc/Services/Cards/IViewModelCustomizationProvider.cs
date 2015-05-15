using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationProvider
    {
        IEnumerable<IViewModelCustomization<TModel>> GetCustomizations<TModel, TEntity>() where TModel : IEntityViewModelBase where TEntity : IEntityKey;
    }
}