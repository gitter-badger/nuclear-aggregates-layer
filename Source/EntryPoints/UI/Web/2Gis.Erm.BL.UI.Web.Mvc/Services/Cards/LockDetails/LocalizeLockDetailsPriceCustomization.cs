using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LockDetails
{
    public sealed class LocalizeLockDetailsPriceCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LockDetailViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                return;
            }

            entityViewModel.Price.Value = entityViewModel.Price.Key.HasValue ? BLResources.PriceN + entityViewModel.Price.Key.Value : null;
        }
    }
}