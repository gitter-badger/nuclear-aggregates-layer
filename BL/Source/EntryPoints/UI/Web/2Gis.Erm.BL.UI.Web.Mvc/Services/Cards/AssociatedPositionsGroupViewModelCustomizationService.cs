using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class AssociatedPositionsGroupViewModelCustomizationService : IGenericViewModelCustomizationService<AssociatedPositionsGroup>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (AssociatedPositionsGroupViewModel)viewModel;
            if (!entityViewModel.IsNew && !entityViewModel.IsDeleted && !entityViewModel.PriceIsPublished && !entityViewModel.PriceIsDeleted)
            {
                return;
            }

            if (entityViewModel.IsNew && entityViewModel.PriceIsPublished)
            {
                entityViewModel.SetInfo(BLResources.CantAddNewGroupWhenPriceIsPublished);
                entityViewModel.ViewConfig.ReadOnly = true;
                return;
            }

            if (entityViewModel.PriceIsPublished)
            {
                entityViewModel.SetInfo(BLResources.CantEditGroupWhenPriceIsPublished);
            }
            else if (entityViewModel.PriceIsDeleted)
            {
                entityViewModel.SetInfo(BLResources.CantEditGroupWhenPriceIsDeactivated);
            }
            else if (entityViewModel.IsDeleted)
            {
                entityViewModel.SetInfo(BLResources.CantEditDeactivatedAssociatedPositionsGroup);
            }
        }
    }
}