using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Accounts
{
    public class AccountIsInactiveCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                return;
            }

            if (viewModel.IsDeleted)
            {
                viewModel.SetCriticalError(BLResources.AccountIsDeletedAlertText);
            }
            else if (!viewModel.IsActive)
            {
                viewModel.SetWarning(BLResources.AccountIsInactiveAlertText);
            }
        }
    }
}