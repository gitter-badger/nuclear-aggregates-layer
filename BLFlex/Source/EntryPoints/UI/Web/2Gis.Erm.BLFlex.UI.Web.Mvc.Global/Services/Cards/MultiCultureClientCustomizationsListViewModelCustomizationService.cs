using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    // TODO {d.ivanov, 02.09.2014}: см коммент для RussiaClientCustomizationsListViewModelCustomizationService
    public class MultiCultureClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>,
                                                                   ICyprusAdapted,
                                                                   ICzechAdapted,
                                                                   IChileAdapted,
                                                                   IUkraineAdapted,
                                                                   IEmiratesAdapted
    {
        private readonly IEnumerable<IViewModelCustomization> _customizations;

        public MultiCultureClientViewModelCustomizationService(IEnumerable<IViewModelCustomization> customizations)
        {
            _customizations = customizations;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            foreach (var customization in _customizations)
            {
                customization.Customize(viewModel);
            }
        }
    }
}