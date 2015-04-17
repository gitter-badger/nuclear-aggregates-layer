using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.LegalPersons
{
    public sealed class LegalPersonIsInactiveCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsDeleted)
            {
                viewModel.SetCriticalError(BLResources.LegalPersonIsDeletedAlertText);
            }
            else if (!viewModel.IsActive)
            {
                viewModel.SetWarning(BLResources.LegalPersonIsInactiveAlertText);
            }
        }
    }
}