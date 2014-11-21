using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderProcessingRequests
{
    public sealed class ManageRequestStateButtonsCustomization : IViewModelCustomization<OrderProcessingRequestViewModel>
    {
        public void Customize(OrderProcessingRequestViewModel viewModel, ModelStateDictionary modelState)
        {
            var deadEndStates = new[]
                {
                    OrderProcessingRequestState.Completed,
                    OrderProcessingRequestState.Cancelled
                };

            if (deadEndStates.Contains(viewModel.State))
            {
                viewModel.ViewConfig.DisableCardToolbarItem("CancelOrderProcessingRequest");
                viewModel.ViewConfig.DisableCardToolbarItem("CreateOrder");
            }
        }
    }
}