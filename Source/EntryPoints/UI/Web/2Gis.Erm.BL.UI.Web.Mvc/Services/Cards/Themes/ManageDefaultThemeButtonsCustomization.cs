using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Themes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Themes
{
    public sealed class ManageDefaultThemeButtonsCustomization : IViewModelCustomization
    {
        private readonly IThemeRepository _themeRepository;

        public ManageDefaultThemeButtonsCustomization(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ThemeViewModel)viewModel;
            var themeCanBeSetAsDefault = _themeRepository.CanThemeBeDefault(entityViewModel.Id);

            var buttonSetDefault = entityViewModel.ViewConfig.CardSettings.CardToolbar
                .Single(item => string.Equals(item.Name, "SetDefaultTheme", StringComparison.Ordinal));
            buttonSetDefault.Disabled = buttonSetDefault.Disabled || entityViewModel.IsDefault || !themeCanBeSetAsDefault;

            var buttonUnSetDefault = entityViewModel.ViewConfig.CardSettings.CardToolbar
                .Single(item => string.Equals(item.Name, "UnSetDefaultTheme", StringComparison.Ordinal));
            buttonUnSetDefault.Disabled = buttonUnSetDefault.Disabled || !entityViewModel.IsDefault;
        }
    }
}