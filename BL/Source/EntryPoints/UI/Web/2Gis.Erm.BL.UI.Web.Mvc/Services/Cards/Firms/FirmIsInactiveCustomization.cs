using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Firms
{
    public sealed class FirmIsInactiveCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        public void Customize(IEntityViewModelBase entityViewModel, ModelStateDictionary modelState)
        {
            if (entityViewModel.IsDeleted)
            {
                entityViewModel.SetCriticalError(BLResources.FirmIsDeletedAlertText);
            }
            else if (!entityViewModel.IsActive)
            {
                entityViewModel.SetWarning(BLResources.FirmIsInactiveAlertText);
            }
            else if (((IClosedForAscertainmentAspect)entityViewModel).ClosedForAscertainment)
            {
                entityViewModel.SetWarning(BLResources.FirmIsClosedForAscertainmentAlertText);
            }
        }
    }
}