using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Locks
{
    public sealed class NewLockCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                viewModel.SetCriticalError(BLResources.CreateOrEditLockFromUINotSupported);
            }
        }
    }
}