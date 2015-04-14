using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class InactiveOrderCustomization : IViewModelCustomization<EntityViewModelBase<Order>>
    {
        public void Customize(EntityViewModelBase<Order> viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                return;
            }

            if (!viewModel.IsActive)
            {
                viewModel.LockOrderToolbar();
                viewModel.SetWarning(BLResources.WarningOrderIsRejected);
            }
        }
    }
}