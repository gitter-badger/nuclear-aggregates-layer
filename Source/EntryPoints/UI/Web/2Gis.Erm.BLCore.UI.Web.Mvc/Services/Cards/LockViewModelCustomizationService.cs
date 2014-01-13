using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class LockViewModelCustomizationService : IGenericViewModelCustomizationService<Lock>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LockViewModel)viewModel;
            if (entityViewModel.IsNew)
            {
                entityViewModel.SetCriticalError(BLResources.CreateOrEditLockFromUINotSupported);
            }
            else
            {
                entityViewModel.Status = entityViewModel.IsActive ? BLResources.Active : BLResources.NotActive;
            }

            entityViewModel.ViewConfig.ReadOnly = true;
        }
    }
}