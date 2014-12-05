using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class MoneySignificantDigitsNumberCustomization : IViewModelCustomization<ICustomizableOrderPositionViewModel>
    {
        private readonly IBusinessModelSettings _businessModelSettings;

        public MoneySignificantDigitsNumberCustomization(IBusinessModelSettings businessModelSettings)
        {
            _businessModelSettings = businessModelSettings;
        }

        public void Customize(ICustomizableOrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.MoneySignificantDigitsNumber = _businessModelSettings.SignificantDigitsNumber;
        }
    }
}