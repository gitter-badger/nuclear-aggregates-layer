using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class OrderFileViewModelCustomizationService : IGenericViewModelCustomizationService<OrderFile>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var orderFileModel = (OrderFileViewModel)viewModel;

            orderFileModel.ViewConfig.ReadOnly |= orderFileModel.UserDoesntHaveRightsToEditOrder;
        }
    }
}