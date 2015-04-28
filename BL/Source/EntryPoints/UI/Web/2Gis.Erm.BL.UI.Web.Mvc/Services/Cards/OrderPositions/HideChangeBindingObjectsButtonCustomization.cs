using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    // TODO {all, 11.03.2015}: Удалить
    public sealed class HideChangeBindingObjectsButtonCustomization : IViewModelCustomization<OrderPositionViewModel>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICanChangeOrderPositionBindingObjectsDetector _canChangeOrderPositionBindingObjectsDetector;

        public HideChangeBindingObjectsButtonCustomization(IOrderReadModel orderReadModel,
                                                           ICanChangeOrderPositionBindingObjectsDetector canChangeOrderPositionBindingObjectsDetector)
        {
            _orderReadModel = orderReadModel;
            _canChangeOrderPositionBindingObjectsDetector = canChangeOrderPositionBindingObjectsDetector;
        }

        public void Customize(OrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew || !IsBindingObjectChangeAllowed(viewModel.Id))
            {
                viewModel.ViewConfig.CardSettings.CardToolbar
                    = viewModel.ViewConfig.CardSettings.CardToolbar
                               .Where(x => !string.Equals(x.Name, "ChangeBindingObjects", StringComparison.Ordinal))
                               .ToArray();
            }
        }

        private bool IsBindingObjectChangeAllowed(long orderPositionId)
        {
            var orderPositionAdvertisementLinksInfo = _orderReadModel.GetOrderPositionAdvertisementLinksInfo(orderPositionId);

            string report;
            return _canChangeOrderPositionBindingObjectsDetector.CanChange(orderPositionAdvertisementLinksInfo.OrderWorkflowState,
                                                                           orderPositionAdvertisementLinksInfo.BindingType,
                                                                           true,
                                                                           null,
                                                                           null,
                                                                           out report);
        }
    }
}