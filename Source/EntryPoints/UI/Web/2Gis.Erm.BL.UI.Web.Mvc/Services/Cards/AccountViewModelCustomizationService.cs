using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class AccountViewModelCustomizationService : IGenericViewModelCustomizationService<Account>, IRussiaAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (AccountViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                return;
            }

            if (entityViewModel.IsDeleted || !entityViewModel.IsActive)
            {
                entityViewModel.MessageType = (MessageType)(entityViewModel.IsDeleted
                                                  ? (int)MessageType.CriticalError
                                                  : !entityViewModel.IsActive ? (int)MessageType.Warning : (int)MessageType.None);

                entityViewModel.Message = entityViewModel.IsDeleted
                                    ? BLResources.AccountIsDeletedAlertText
                                    : !entityViewModel.IsActive ? BLResources.AccountIsInactiveAlertText : string.Empty;
            }
        }
    }
}