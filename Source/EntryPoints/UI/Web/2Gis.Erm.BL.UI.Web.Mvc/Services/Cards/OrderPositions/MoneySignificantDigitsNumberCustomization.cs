using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public class MoneySignificantDigitsNumberCustomization : IViewModelCustomization
    {
        private readonly IBusinessModelSettings _businessModelSettings;

        public MoneySignificantDigitsNumberCustomization(IBusinessModelSettings businessModelSettings)
        {
            _businessModelSettings = businessModelSettings;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (IOrderPositionViewModel)viewModel;

            entityViewModel.MoneySignificantDigitsNumber = _businessModelSettings.SignificantDigitsNumber;
        }
    }
}