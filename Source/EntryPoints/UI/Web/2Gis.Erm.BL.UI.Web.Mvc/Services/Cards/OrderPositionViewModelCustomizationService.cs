using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class OrderPositionViewModelCustomizationService : IGenericViewModelCustomizationService<OrderPosition>
    {
        private readonly IPublicService _publicService;
        private readonly IBusinessModelSettings _businessModelSettings;

        public OrderPositionViewModelCustomizationService(IPublicService publicService, IBusinessModelSettings businessModelSettings)
        {
            _publicService = publicService;
            _businessModelSettings = businessModelSettings;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (OrderPositionViewModel)viewModel;

            entityViewModel.MoneySignificantDigitsNumber = _businessModelSettings.SignificantDigitsNumber;
            
            // логика удаления кнопки не вписывается в стандартную схему.
            var checkResponse = (CheckIsBindingObjectChangeAllowedResponse)
                                _publicService.Handle(new CheckIsBindingObjectChangeAllowedRequest
                                    {
                                        SkipAdvertisementCountCheck = true,
                                        OrderPositionId = entityViewModel.Id,
                                    });

            if (entityViewModel.IsNew)
            {
                entityViewModel.DiscountPercent = ((GetInitialDiscountResponse)_publicService.Handle(new GetInitialDiscountRequest
                    {
                        OrderId = entityViewModel.OrderId
                    }))
                    .DiscountPercent;
            }

            if (!checkResponse.IsChangeAllowed)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar = entityViewModel.ViewConfig.CardSettings.CardToolbar
                                                                 .Where(x => !string.Equals(x.Name, "ChangeBindingObjects", StringComparison.Ordinal))
                                                                 .ToArray();
            }

            if (!entityViewModel.IsNew && entityViewModel.IsRated && entityViewModel.CategoryRate != 1 && string.IsNullOrEmpty(entityViewModel.Message))
            {
                // приведение к double используется, чтобы отбросить информацию о формате, хранящуюся в decimal и не выводить незначащие нули справа
                entityViewModel.Message = string.Format(BLResources.CategoryGroupInfoMessage, (double)entityViewModel.CategoryRate);
                entityViewModel.MessageType = MessageType.Info;
            }
        }
    }
}