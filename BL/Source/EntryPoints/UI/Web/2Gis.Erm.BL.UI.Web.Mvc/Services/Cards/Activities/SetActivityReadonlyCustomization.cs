using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Activities
{
    public sealed class SetActivityReadonlyCustomization : IViewModelCustomization<ICustomizableActivityViewModel>
    {
        public void Customize(ICustomizableActivityViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.Status == ActivityStatus.Canceled || viewModel.Status == ActivityStatus.Completed)
            {
                viewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}