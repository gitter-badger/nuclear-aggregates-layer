using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements
{
    public class ManageAdvertisementElementWorkflowButtonsCustomizations : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementElementModel = (AdvertisementElementViewModel)viewModel;

            if (advertisementElementModel.CanUserChangeStatus || advertisementElementModel.DisableEdit)
            {
                advertisementElementModel.ViewConfig.DisableCardToolbarItem("ResetToDraft");
            }

            advertisementElementModel.ViewConfig.DisableCardToolbarItem(advertisementElementModel.Status == AdvertisementElementStatusValue.Draft
                                                                            ? "ResetToDraft"
                                                                            : "SaveAndVerify");

            var itemsToDelete = advertisementElementModel.NeedsValidation ? new[] { "Save", "SaveAndClose" } : new[] { "ResetToDraft", "SaveAndVerify" };

            advertisementElementModel.ViewConfig.CardSettings.CardToolbar =
                advertisementElementModel.ViewConfig.CardSettings.CardToolbar.Where(x => !itemsToDelete.Contains(x.Name)).ToArray();
        }
    }
}
