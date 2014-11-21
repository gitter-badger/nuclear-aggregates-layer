using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class ReleaseInfoViewModelCustomizationService : IGenericViewModelCustomizationService<ReleaseInfo>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ReleaseInfoViewModel)viewModel;
            entityViewModel.ViewConfig.ReadOnly = true;
        }
    }
}