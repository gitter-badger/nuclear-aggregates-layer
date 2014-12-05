using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderFiles
{
    public sealed class OrderFileAccessCustomization : IViewModelCustomization<OrderFileViewModel>
    {
        public void Customize(OrderFileViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.ViewConfig.ReadOnly |= viewModel.UserDoesntHaveRightsToEditOrder;
        }
    }
}