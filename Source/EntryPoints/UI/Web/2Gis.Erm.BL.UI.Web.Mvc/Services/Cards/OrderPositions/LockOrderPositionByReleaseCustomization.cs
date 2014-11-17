using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class LockOrderPositionByReleaseCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderPositionViewModel)viewModel;

            if (entityViewModel.IsBlockedByRelease)
            {
                entityViewModel.SetWarning(BLResources.CannotEditOrderPositionSinceReleaseIsInProgress);
            }
        }
    }
}