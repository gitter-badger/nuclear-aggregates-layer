using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.UI.Web.Mvc.Models;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

using MessageType = DoubleGis.Erm.Platform.Web.Mvc.ViewModel.MessageType;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    public class CyprusFirmViewModelCustomizationService : IGenericViewModelCustomizationService<Firm>, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public CyprusFirmViewModelCustomizationService(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (CyprusFirmViewModel)viewModel;

            // Наличие кнопки "Сменить территорию" зависит от наличия у пользователя привелегии на этьо действие.
            var hasPrivelege = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ChangeFirmTerritory,
                                                                                      _userContext.Identity.Code);
            if (!hasPrivelege)
            {
                entityViewModel.ViewConfig.CardSettings.CardToolbar =
                    entityViewModel.ViewConfig.CardSettings.CardToolbar
                                   .Where(x => !string.Equals(x.Name, "ChangeTerritory", StringComparison.Ordinal))
                                   .ToArray();
            }

            if (entityViewModel.IsNew)
            {
                return;
            }

            entityViewModel.MessageType = (MessageType)(entityViewModel.IsDeleted
                                                            ? (int)MessageType.CriticalError
                                                            : !entityViewModel.IsActive || entityViewModel.ClosedForAscertainment
                                                                  ? (int)MessageType.Warning
                                                                  : (int)MessageType.None);

            entityViewModel.Message = entityViewModel.IsDeleted
                                          ? BLResources.FirmIsDeletedAlertText
                                          : !entityViewModel.IsActive
                                                ? BLResources.FirmIsInactiveAlertText
                                                : entityViewModel.ClosedForAscertainment
                                                      ? BLResources.FirmIsClosedForAscertainmentAlertText
                                                      : string.Empty;
        }
    }
}