using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class OrderPositionRateCustomization : IViewModelCustomization<OrderPositionViewModel>
    {
        public void Customize(OrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                return;
            }

            if (viewModel.IsRated && viewModel.CategoryRate != 1 && string.IsNullOrEmpty(viewModel.Message))
            {
                // приведение к double используется, чтобы отбросить информацию о формате, хранящуюся в decimal и не выводить незначащие нули справа
                viewModel.SetInfo(string.Format(BLResources.CategoryGroupInfoMessage, (double)viewModel.CategoryRate));
            }
        }
    }
}