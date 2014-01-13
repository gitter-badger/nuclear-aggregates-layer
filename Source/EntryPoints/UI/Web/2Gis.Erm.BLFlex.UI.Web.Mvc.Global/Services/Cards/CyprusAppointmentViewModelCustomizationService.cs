using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class CyprusAppointmentViewModelCustomizationService : IGenericViewModelCustomizationService<Appointment>, ICyprusAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CyprusActivityViewModelCustomizationServiceHelper.CustomizeViewModel<Appointment>(viewModel, modelState);
        }
    }
}