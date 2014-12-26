using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositionGroups
{
    public sealed class AssociatedPositionGroupsPriceIsPublishedCustomization : IViewModelCustomization<AssociatedPositionsGroupViewModel>
    {
        public void Customize(AssociatedPositionsGroupViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.PriceIsPublished)
            {
                viewModel.SetInfo(viewModel.IsNew
                                      ? BLResources.CantAddNewGroupWhenPriceIsPublished
                                      : BLResources.CantEditGroupWhenPriceIsPublished);
            }
        }
    }
}
