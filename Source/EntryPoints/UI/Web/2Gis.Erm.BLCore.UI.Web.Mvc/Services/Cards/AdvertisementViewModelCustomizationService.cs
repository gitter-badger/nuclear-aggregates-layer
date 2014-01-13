using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class AdvertisementViewModelCustomizationService : IGenericViewModelCustomizationService<Advertisement>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementModel = (AdvertisementViewModel)viewModel;
            if (advertisementModel.IsSelectedToWhiteList)
            {
                advertisementModel.SetInfo(BLResources.AdvertisementIsSelectedToWhiteList);
            }

            if (advertisementModel.IsDummy)
            {
                advertisementModel.ViewConfig.ReadOnly = true;
            }
        }
    }
}