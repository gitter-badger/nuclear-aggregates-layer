using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class CzechLegalPersonProfileViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPersonProfile>, ICzechAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (CzechLegalPersonProfileViewModel)viewModel;

            if (entityViewModel.IsMainProfile)
            {
                entityViewModel.Message = BLResources.LegalPersonProfileIsMain;
                entityViewModel.MessageType = MessageType.Info;
            }

            entityViewModel.DisabledDocuments = new int[0];
        }
    }
}