using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationService<in TModel> : IUIService
        where TModel : IEntityViewModelBase
    {
        void CustomizeViewModel(TModel viewModel, ModelStateDictionary modelState);
    }
}