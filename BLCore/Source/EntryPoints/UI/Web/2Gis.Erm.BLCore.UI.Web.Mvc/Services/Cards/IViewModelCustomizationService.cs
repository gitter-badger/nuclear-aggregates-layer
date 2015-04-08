using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationService : IUIService
    {
        void CustomizeViewModel<TModel, TEntity>(TModel viewModel, ModelStateDictionary modelState)
            where TModel : IEntityViewModelBase
            where TEntity : IEntityKey;
    }
}