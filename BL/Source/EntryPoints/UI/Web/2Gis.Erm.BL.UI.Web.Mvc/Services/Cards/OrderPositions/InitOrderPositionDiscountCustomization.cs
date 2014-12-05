using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class InitOrderPositionDiscountCustomization : IViewModelCustomization<ICustomizableOrderPositionViewModel>
    {
        private readonly IPublicService _publicService;

        public InitOrderPositionDiscountCustomization(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public void Customize(ICustomizableOrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                viewModel.DiscountPercent = ((GetInitialDiscountResponse)
                                             _publicService.Handle(new GetInitialDiscountRequest
                                                                       {
                                                                           OrderId = viewModel.OrderId
                                                                       }))
                    .DiscountPercent;
            }
        }
    }
}