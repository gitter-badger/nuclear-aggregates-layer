using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class MultiCultureOrderPositionViewModelCustomizationService : IGenericViewModelCustomizationService<OrderPosition>
    {
        private readonly IPublicService _publicService;
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICanChangeOrderPositionBindingObjectsDetector _canChangeOrderPositionBindingObjectsDetector;

        public MultiCultureOrderPositionViewModelCustomizationService(
            IBusinessModelSettings businessModelSettings,
            IOrderReadModel orderReadModel,
            ICanChangeOrderPositionBindingObjectsDetector canChangeOrderPositionBindingObjectsDetector,
            IPublicService publicService)
        {
            _publicService = publicService;
            _businessModelSettings = businessModelSettings;
            _orderReadModel = orderReadModel;
            _canChangeOrderPositionBindingObjectsDetector = canChangeOrderPositionBindingObjectsDetector;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (MultiCultureOrderPositionViewModel)viewModel;

            entityViewModel.MoneySignificantDigitsNumber = _businessModelSettings.SignificantDigitsNumber;

            if (entityViewModel.IsNew)
            {
                entityViewModel.DiscountPercent = ((GetInitialDiscountResponse)_publicService.Handle(new GetInitialDiscountRequest
                    {
                        OrderId = entityViewModel.OrderId
                    }))
                    .DiscountPercent;
            }

            if (entityViewModel.IsNew || !IsBindingObjectChangeAllowed(entityViewModel.Id))
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar = entityViewModel.ViewConfig.CardSettings.CardToolbar
                                                                                     .Where(
                                                                                         x =>
                                                                                         !string.Equals(x.Name, "ChangeBindingObjects", StringComparison.Ordinal))
                                                                                     .ToArray();
            }

            if (!entityViewModel.IsNew && entityViewModel.IsRated && entityViewModel.CategoryRate != 1 && string.IsNullOrEmpty(entityViewModel.Message))
            {
                // приведение к double используется, чтобы отбросить информацию о формате, хранящуюся в decimal и не выводить незначащие нули справа
                entityViewModel.Message = string.Format(BLResources.CategoryGroupInfoMessage, (double)entityViewModel.CategoryRate);
                entityViewModel.MessageType = MessageType.Info;
            }

            if (entityViewModel.IsBlockedByRelease)
            {
                entityViewModel.SetWarning(BLResources.CannotEditOrderPositionSinceReleaseIsInProgress);
            }
        }

        private bool IsBindingObjectChangeAllowed(long orderPositionId)
        {
            var orderPositionAdvertisementLinksInfo = _orderReadModel.GetOrderPositionAdvertisementLinksInfo(orderPositionId);

            string report;
            return _canChangeOrderPositionBindingObjectsDetector.CanChange(
                orderPositionAdvertisementLinksInfo.OrderWorkflowState,
                orderPositionAdvertisementLinksInfo.BindingType,
                true,
                null,
                null,
                out report);
        }
    }
}