using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class HideChangeBindingObjectsButtonCustomization : IViewModelCustomization<ICustomizableOrderPositionViewModel>
    {
        private readonly IPublicService _publicService;

        public HideChangeBindingObjectsButtonCustomization(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public void Customize(ICustomizableOrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            var checkResponse = (CheckIsBindingObjectChangeAllowedResponse)
                                _publicService.Handle(new CheckIsBindingObjectChangeAllowedRequest
                                                          {
                                                              SkipAdvertisementCountCheck = true,
                                                              OrderPositionId = viewModel.Id,
                                                          });

            if (!checkResponse.IsChangeAllowed)
            {
                viewModel.ViewConfig.CardSettings.CardToolbar
                    = viewModel.ViewConfig.CardSettings.CardToolbar
                               .Where(x => !string.Equals(x.Name, "ChangeBindingObjects", StringComparison.Ordinal))
                               .ToArray();
            }
        }
    }
}