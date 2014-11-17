using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class InactiveOrderCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                return;
            }

            if (!entityViewModel.IsActive)
            {
                entityViewModel.LockToolbar();
                entityViewModel.SetWarning(BLResources.WarningOrderIsRejected);
                entityViewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}