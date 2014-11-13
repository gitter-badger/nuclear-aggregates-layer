using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public class HideChangeBindingObjectsButtonCustomization : IViewModelCustomization
    {
        private readonly IPublicService _publicService;

        public HideChangeBindingObjectsButtonCustomization(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderPositionViewModel)viewModel;

            var checkResponse = (CheckIsBindingObjectChangeAllowedResponse)
                                _publicService.Handle(new CheckIsBindingObjectChangeAllowedRequest
                                {
                                    SkipAdvertisementCountCheck = true,
                                    OrderPositionId = entityViewModel.Id,
                                });

            if (!checkResponse.IsChangeAllowed)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar = entityViewModel.ViewConfig.CardSettings.CardToolbar
                                                                                     .Where(x =>
                                                                                            !string.Equals(x.Name, "ChangeBindingObjects", StringComparison.Ordinal))
                                                                                     .ToArray();
            }
        }
    }
}