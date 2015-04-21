using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomization
    {
    }

    public interface IViewModelCustomization<in TModel> : IViewModelCustomization
        where TModel : IEntityViewModelBase
    {
        void Customize(TModel viewModel, ModelStateDictionary modelState);
    }
}