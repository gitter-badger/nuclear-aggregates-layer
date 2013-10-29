using System.Web.Mvc;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.UI.Web.Mvc.Models;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

using MessageType = DoubleGis.Erm.Platform.Web.Mvc.ViewModel.MessageType;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    public class LegalPersonViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPerson>, IRussiaAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LegalPersonViewModel)viewModel;

            if (entityViewModel.IsNew)
            {
                return;
            }

            if (entityViewModel.IsDeleted || !entityViewModel.IsActive)
            {
                entityViewModel.MessageType = (MessageType)(entityViewModel.IsDeleted
                                                  ? (int)MessageType.CriticalError
                                                  : !entityViewModel.IsActive ? (int)MessageType.Warning : (int)MessageType.None);

                entityViewModel.Message = entityViewModel.IsDeleted
                                    ? BLResources.LegalPersonIsDeletedAlertText
                                    : !entityViewModel.IsActive ? BLResources.LegalPersonIsInactiveAlertText : string.Empty;
            }

            if (!entityViewModel.HasProfiles)
            {
                entityViewModel.SetWarning(BLResources.MustMakeLegalPersonProfile);
            }
        }
    }
}