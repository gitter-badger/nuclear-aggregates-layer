using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class FirmContactViewModelCustomizationService : IGenericViewModelCustomizationService<FirmContact>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (FirmContactViewModel)viewModel;
            entityViewModel.ViewConfig.ReadOnly = true;
        }
    }
}