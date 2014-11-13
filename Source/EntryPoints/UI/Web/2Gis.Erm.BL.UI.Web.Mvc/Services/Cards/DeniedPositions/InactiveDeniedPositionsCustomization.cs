using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.DeniedPositions
{
    public class InactiveDeniedPositionsCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (DeniedPositionViewModel)viewModel;
            if (!entityViewModel.IsDeleted)
            {
                return;
            }

            entityViewModel.SetInfo(BLResources.CantEditDeactivatedDeniedPosition);
            entityViewModel.ViewConfig.ReadOnly = true;
        }
    }
}