using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositions
{
    public sealed class AssociatedPositionsPriceIsPublishedCustomization : IViewModelCustomization<AssociatedPositionViewModel>
    {
        public void Customize(AssociatedPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.PriceIsPublished)
            {
                viewModel.SetInfo(viewModel.IsNew
                                      ? BLResources.CantAddAssociatedPositionToGroupWhenPriceIsPublished
                                      : BLResources.CantEditAssociatedPositionInGroupWhenPriceIsPublished);
            }
        }
    }
}
