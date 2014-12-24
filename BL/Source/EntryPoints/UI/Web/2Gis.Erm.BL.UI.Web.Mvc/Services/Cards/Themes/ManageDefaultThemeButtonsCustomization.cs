using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Themes
{
    public sealed class ManageDefaultThemeButtonsCustomization : IViewModelCustomization<ThemeViewModel>
    {
        private readonly IThemeRepository _themeRepository;

        public ManageDefaultThemeButtonsCustomization(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }

        public void Customize(ThemeViewModel viewModel, ModelStateDictionary modelState)
        {
            var themeCanBeSetAsDefault = _themeRepository.CanThemeBeDefault(viewModel.Id);

            var buttonSetDefault = viewModel.ViewConfig.CardSettings.CardToolbar
                .Single(item => string.Equals(item.Name, "SetDefaultTheme", StringComparison.Ordinal));
            buttonSetDefault.Disabled = buttonSetDefault.Disabled || viewModel.IsDefault || !themeCanBeSetAsDefault;

            var buttonUnSetDefault = viewModel.ViewConfig.CardSettings.CardToolbar
                .Single(item => string.Equals(item.Name, "UnSetDefaultTheme", StringComparison.Ordinal));
            buttonUnSetDefault.Disabled = buttonUnSetDefault.Disabled || !viewModel.IsDefault;
        }
    }
}