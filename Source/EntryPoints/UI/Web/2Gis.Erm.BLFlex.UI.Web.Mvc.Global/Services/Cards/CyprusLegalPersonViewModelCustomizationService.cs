using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class CyprusLegalPersonViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPerson>, ICyprusAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (CyprusLegalPersonViewModel)viewModel;

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