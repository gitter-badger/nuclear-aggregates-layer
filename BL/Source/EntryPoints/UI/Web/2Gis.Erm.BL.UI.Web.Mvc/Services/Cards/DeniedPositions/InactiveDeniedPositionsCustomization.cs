using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.DeniedPositions
{
    public sealed class InactiveDeniedPositionsCustomization : IViewModelCustomization<DeniedPositionViewModel>
    {
        public void Customize(DeniedPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsDeleted)
            {
                viewModel.SetInfo(BLResources.CantEditDeactivatedDeniedPosition);
            }
        }
    }
}