using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class LockOrderPositionByReleaseCustomization : IViewModelCustomization<OrderPositionViewModel>
    {
        public void Customize(OrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsBlockedByRelease)
            {
                viewModel.SetWarning(BLResources.CannotEditOrderPositionSinceReleaseIsInProgress);
            }
        }
    }
}