using System.Web.Mvc;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    public class CzechTaskViewModelCustomizationService : IGenericViewModelCustomizationService<Task>, ICzechAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CzechActivityViewModelCustomizationServiceHelper.CustomizeViewModel<Task>(viewModel, modelState);
        }
    }
}