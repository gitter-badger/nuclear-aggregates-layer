using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LockDetails
{
    public sealed class NewLockDetailCustomization : IViewModelCustomization<LockDetailViewModel>
    {
        public void Customize(LockDetailViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                viewModel = new LockDetailViewModel { ViewConfig = { ReadOnly = true } };
                viewModel.SetCriticalError(BLResources.CreateOrEditLockDetailFromUINotSupported);
            }
        }
    }
}