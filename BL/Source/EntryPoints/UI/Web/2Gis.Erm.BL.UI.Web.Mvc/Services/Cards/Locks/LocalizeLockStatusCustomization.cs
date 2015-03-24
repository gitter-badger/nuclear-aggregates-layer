using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Locks
{
    public sealed class LocalizeLockStatusCustomization : IViewModelCustomization<LockViewModel>
    {
        public void Customize(LockViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.Status = viewModel.IsActive ? BLResources.Active : BLResources.NotActive;
        }
    }
}