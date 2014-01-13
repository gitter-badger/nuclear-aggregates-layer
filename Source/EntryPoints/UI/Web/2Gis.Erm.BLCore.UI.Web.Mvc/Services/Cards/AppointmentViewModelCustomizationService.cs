using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class AppointmentViewModelCustomizationService : IGenericViewModelCustomizationService<Appointment>, IRussiaAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            ActivityViewModelCustomizationServiceHelper.CustomizeViewModel<Appointment>(viewModel, modelState);
        }
    }
}