using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class LockByWorkflowCustomization : IViewModelCustomization<ICustomizableOrderViewModel>
    {
        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.ViewConfig.ReadOnly |= viewModel.WorkflowStepId != (int)OrderState.OnRegistration;
        }
    }
}