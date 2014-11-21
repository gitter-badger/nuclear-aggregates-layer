using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class UserViewModelCustomizationService : IGenericViewModelCustomizationService<User>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (UserViewModel)viewModel;
            if (!entityViewModel.IsNew && !entityViewModel.IsActive)
            {
                entityViewModel.SetWarning(BLResources.EntityIsInactive);
            }
        }
    }
}