using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class LockDetailViewModelCustomizationService : IGenericViewModelCustomizationService<LockDetail>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LockDetailViewModel)viewModel;

            // todo: implement good price name
            if (entityViewModel.IsNew)
            {
                entityViewModel = new LockDetailViewModel { ViewConfig = { ReadOnly = true } };
                entityViewModel.SetCriticalError(BLResources.CreateOrEditLockDetailFromUINotSupported);
            }
            else
            {
                entityViewModel.Price.Value = entityViewModel.Price.Key.HasValue ? BLResources.PriceN + entityViewModel.Price.Key.Value : null;
            }
        }
    }
}