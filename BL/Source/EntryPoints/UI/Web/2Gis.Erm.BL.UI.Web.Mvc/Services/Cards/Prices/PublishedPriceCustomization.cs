using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Prices
{
    public sealed class PublishedPriceCustomization : IViewModelCustomization<PriceViewModel>
    {
        public void Customize(PriceViewModel viewModel, ModelStateDictionary modelState)
        {
            if (!viewModel.IsPublished)
            {
                return;
            }

            viewModel.SetInfo(BLResources.CantEditPriceWhenPublished);
            viewModel.ViewConfig.ReadOnly = true;
        }
    }
}