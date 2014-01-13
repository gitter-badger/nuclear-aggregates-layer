using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class CzechPhonecallViewModelCustomizationService : IGenericViewModelCustomizationService<Phonecall>, ICzechAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CzechActivityViewModelCustomizationServiceHelper.CustomizeViewModel<Phonecall>(viewModel, modelState);
        }
    }
}