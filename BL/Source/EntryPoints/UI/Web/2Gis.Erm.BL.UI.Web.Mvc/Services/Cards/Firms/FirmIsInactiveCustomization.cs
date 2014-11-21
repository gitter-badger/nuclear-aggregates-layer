using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms
{
    public sealed class FirmIsInactiveCustomization : IViewModelCustomization<ICustomizableFirmViewModel>
    {
        public void Customize(ICustomizableFirmViewModel entityViewModel, ModelStateDictionary modelState)
        {
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