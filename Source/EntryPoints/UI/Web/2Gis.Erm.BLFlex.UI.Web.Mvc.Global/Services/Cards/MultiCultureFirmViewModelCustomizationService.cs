﻿using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class MultiCultureFirmViewModelCustomizationService : IGenericViewModelCustomizationService<Firm>, ICyprusAdapted, IChileAdapted, ICzechAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;

        public MultiCultureFirmViewModelCustomizationService(IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService)
        {
            _userContext = userContext;
            _functionalAccessService = functionalAccessService;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (MultiCultureFirmViewModel)viewModel;

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