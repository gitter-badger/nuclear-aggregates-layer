using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AdvertisementElements
{
    public sealed class ManageAdvertisementElementWorkflowButtonsCustomizations : IViewModelCustomization<AdvertisementElementViewModel>
    {
        public void Customize(AdvertisementElementViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.CanUserChangeStatus || viewModel.DisableEdit)
            {
                viewModel.ViewConfig.DisableCardToolbarItem("ResetToDraft");
            }

            viewModel.ViewConfig.DisableCardToolbarItem(viewModel.Status == AdvertisementElementStatusValue.Draft
                                                            ? "ResetToDraft"
                                                            : "SaveAndVerify");

            var itemsToDelete = viewModel.NeedsValidation ? new[] { "Save", "SaveAndClose" } : new[] { "ResetToDraft", "SaveAndVerify" };

            viewModel.ViewConfig.CardSettings.CardToolbar =
                viewModel.ViewConfig.CardSettings.CardToolbar.Where(x => !itemsToDelete.Contains(x.Name)).ToArray();
        }
    }
}