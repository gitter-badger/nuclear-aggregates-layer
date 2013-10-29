using System.Web.Mvc;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    public class CyprusTaskViewModelCustomizationService : IGenericViewModelCustomizationService<Task>, ICyprusAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CyprusActivityViewModelCustomizationServiceHelper.CustomizeViewModel<Task>(viewModel, modelState);
        }
    }
}