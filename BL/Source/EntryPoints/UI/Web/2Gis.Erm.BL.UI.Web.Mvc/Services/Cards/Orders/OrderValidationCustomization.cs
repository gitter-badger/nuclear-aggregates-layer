using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class OrderValidationCustomization : IViewModelCustomization<ICustomizableOrderViewModel>
    {
        private readonly IAPIOrderValidationServiceSettings _orderValidationServiceSettings;

        private readonly OrderState[] _stepsWithAvailableValidation =
            {
                OrderState.OnRegistration,
                OrderState.OnApproval,
                OrderState.Approved
            };

        public OrderValidationCustomization(IAPIOrderValidationServiceSettings orderValidationServiceSettings)
        {
            _orderValidationServiceSettings = orderValidationServiceSettings;
        }

        public void Customize(ICustomizableOrderViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.OrderValidationServiceUrl = _orderValidationServiceSettings.RestUrl;

            var disableOrderValidation = !_stepsWithAvailableValidation.Contains((OrderState)viewModel.WorkflowStepId);

            if (disableOrderValidation)
            {
                viewModel.ViewConfig.DisableCardToolbarItem("CheckOrder");
            }
            else
            {
                viewModel.ViewConfig.EnableCardToolbarItem("CheckOrder");
            }
        }
    }
}