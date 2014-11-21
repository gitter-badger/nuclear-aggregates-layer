using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Cyprus
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

            if (!entityViewModel.HasProfiles)
            {
                entityViewModel.SetWarning(BLResources.MustMakeLegalPersonProfile);
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
        }
    }
}