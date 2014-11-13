using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Prices
{
    public class PublishedPriceCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var priceViewModel = (PriceViewModel)viewModel;

            if (!priceViewModel.IsPublished)
            {
                return;
            }

            priceViewModel.SetInfo(BLResources.CantEditPriceWhenPublished);
            priceViewModel.ViewConfig.ReadOnly = true;
        }
    }
}