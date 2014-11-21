using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class AssociatedPositionViewModelCustomizationService : IGenericViewModelCustomizationService<AssociatedPosition>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (AssociatedPositionViewModel)viewModel;
            if (!entityViewModel.IsNew)
            {
                if (entityViewModel.PriceIsPublished)
                {
                    entityViewModel.SetInfo(BLResources.CantEditAssociatedPositionInGroupWhenPriceIsPublished);
                    entityViewModel.ViewConfig.ReadOnly = true;
                }

                if (entityViewModel.PriceIsDeleted)
                {
                    entityViewModel.ViewConfig.ReadOnly = true;
                }
            }
            else if (entityViewModel.PriceIsPublished)
            {
                entityViewModel.SetInfo(BLResources.CantAddAssociatedPositionToGroupWhenPriceIsPublished);
                entityViewModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}