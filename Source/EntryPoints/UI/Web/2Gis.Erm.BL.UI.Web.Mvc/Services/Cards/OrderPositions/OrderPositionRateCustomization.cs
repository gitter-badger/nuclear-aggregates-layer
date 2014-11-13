using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public class OrderPositionRateCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderPositionViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                return;
            }

            if (entityViewModel.IsRated && entityViewModel.CategoryRate != 1 && string.IsNullOrEmpty(entityViewModel.Message))
            {
                // приведение к double используется, чтобы отбросить информацию о формате, хранящуюся в decimal и не выводить незначащие нули справа
                entityViewModel.SetInfo(string.Format(BLResources.CategoryGroupInfoMessage, (double)entityViewModel.CategoryRate));
            }
        }
    }
}