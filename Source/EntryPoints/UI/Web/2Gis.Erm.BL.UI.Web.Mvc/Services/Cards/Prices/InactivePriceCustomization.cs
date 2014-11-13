using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Prices
{
    public class InactivePriceCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (!viewModel.IsDeleted)
            {
                return;
            }

            viewModel.SetInfo(BLResources.CantEditPriceWhenDeactivated);
            viewModel.ViewConfig.ReadOnly = true;
        }
    }
}