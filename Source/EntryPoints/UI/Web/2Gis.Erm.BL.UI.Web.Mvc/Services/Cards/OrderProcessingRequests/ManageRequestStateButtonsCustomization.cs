using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderProcessingRequests
{
    public sealed class ManageRequestStateButtonsCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (OrderProcessingRequestViewModel)viewModel;

            var deadEndStates = new[]
                {
                    OrderProcessingRequestState.Completed,
                    OrderProcessingRequestState.Cancelled
                };

            if (deadEndStates.Contains(entityViewModel.State))
            {
                entityViewModel.ViewConfig.DisableCardToolbarItem("CancelOrderProcessingRequest");
                entityViewModel.ViewConfig.DisableCardToolbarItem("CreateOrder");
            }
        }
    }
}