using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements
{
    public sealed class CheckIfAdvertisementElementReadOnly : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementElementModel = (AdvertisementElementViewModel)viewModel;

            advertisementElementModel.ViewConfig.ReadOnly |=
                advertisementElementModel.DisableEdit ||
                advertisementElementModel.CanUserChangeStatus ||
                (advertisementElementModel.NeedsValidation && advertisementElementModel.Status != AdvertisementElementStatusValue.Draft);
        }
    }
}
