using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements
{
    public sealed class CheckIfAdvertisementElementReadOnly : IViewModelCustomization<AdvertisementElementViewModel>
    {
        public void Customize(AdvertisementElementViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.ViewConfig.ReadOnly |=
                viewModel.DisableEdit ||
                viewModel.CanUserChangeStatus ||
                (viewModel.NeedsValidation && viewModel.Status != AdvertisementElementStatusValue.Draft);
        }
    }
}