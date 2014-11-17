using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class InitOrderPositionDiscountCustomization : IViewModelCustomization
    {
        private readonly IPublicService _publicService;

        public InitOrderPositionDiscountCustomization(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderPositionViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                entityViewModel.DiscountPercent = ((GetInitialDiscountResponse)
                                                   _publicService.Handle(new GetInitialDiscountRequest
                                                                             {
                                                                                 OrderId = entityViewModel.OrderId
                                                                             }))
                    .DiscountPercent;
            }
        }
    }
}