using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderFiles
{
    public class OrderFileAccessCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var orderFileModel = (OrderFileViewModel)viewModel;

            orderFileModel.ViewConfig.ReadOnly |= orderFileModel.UserDoesntHaveRightsToEditOrder;
        }
    }
}