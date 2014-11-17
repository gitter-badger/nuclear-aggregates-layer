using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms
{
    public sealed class FirmIsInactiveCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IFirmViewModel)viewModel;

            if (entityViewModel.IsDeleted)
            {
                entityViewModel.SetCriticalError(BLResources.FirmIsDeletedAlertText);
            }
            else if (!entityViewModel.IsActive)
            {
                entityViewModel.SetWarning(BLResources.FirmIsInactiveAlertText);
            }
            else if (entityViewModel.ClosedForAscertainment)
            {
                entityViewModel.SetWarning(BLResources.FirmIsClosedForAscertainmentAlertText);
            }
        }
    }
}