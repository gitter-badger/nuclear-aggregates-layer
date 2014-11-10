using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomization
    {
        void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState);
    }
}