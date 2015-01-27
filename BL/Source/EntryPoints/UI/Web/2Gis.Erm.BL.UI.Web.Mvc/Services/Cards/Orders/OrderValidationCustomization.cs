using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class OrderValidationCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
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

        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            ((IOrderValidationServiceAspect)viewModel).OrderValidationServiceUrl = _orderValidationServiceSettings.RestUrl;

            var disableOrderValidation = !_stepsWithAvailableValidation.Contains(((IOrderWorkflowAspect)viewModel).WorkflowStepId);

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