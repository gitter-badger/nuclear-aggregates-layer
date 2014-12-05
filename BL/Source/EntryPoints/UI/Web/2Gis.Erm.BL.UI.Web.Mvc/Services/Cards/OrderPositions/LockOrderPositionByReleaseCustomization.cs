using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class LockOrderPositionByReleaseCustomization : IViewModelCustomization<ICustomizableOrderPositionViewModel>
    {
        public void Customize(ICustomizableOrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsBlockedByRelease)
            {
                viewModel.SetWarning(BLResources.CannotEditOrderPositionSinceReleaseIsInProgress);
            }
        }
    }
}