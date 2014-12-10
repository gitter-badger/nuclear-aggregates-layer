﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class TestController : ControllerBase
    {
        private readonly EntityName[] _entitiesToIgnore =
            {
                EntityName.None,
                EntityName.All,
                EntityName.UserRole,
                EntityName.CategoryOrganizationUnit,
                EntityName.Activity,
                EntityName.FileWithContent,
                EntityName.AdvertisementElementDenialReason,
                EntityName.BirthdayCongratulation,
                EntityName.Commune,
                EntityName.NotificationEmailAttachment,
                EntityName.NotificationEmailTo,
                EntityName.NotificationEmailCc,
                EntityName.NotificationAddress,
                EntityName.NotificationEmail,
                EntityName.NotificationProcessing,
                EntityName.OrderValidationCacheEntry,
                EntityName.PerformedOperationFinalProcessing,
                EntityName.PerformedOperationPrimaryProcessing,
                EntityName.ExportFlowOrdersDenialReason,
                EntityName.ExportFlowDeliveryDataLetterSendRequest,
                EntityName.ExportFlowNomenclaturesNomenclatureElementRelation,
                EntityName.ExportFlowNomenclaturesNomenclatureElement,
                EntityName.ExportFlowOrdersInvoice,
                EntityName.UkraineLegalPersonProfilePart,
                EntityName.ExportFlowPriceListsPriceListPosition,
                EntityName.ExportFlowPriceListsPriceList,
                EntityName.ChileBranchOfficeOrganizationUnitPart,
                EntityName.ChileLegalPersonProfilePart,
                EntityName.HotClientRequest,
                EntityName.ExportFlowFinancialDataClient,
                EntityName.ExportFailedEntity,
                EntityName.ImportedFirmAddress,
                EntityName.ExportFlowOrdersThemeBranch,
                EntityName.ExportFlowOrdersTheme,
                EntityName.ExportFlowOrdersResource,
                EntityName.ExportFlowOrdersOrder,
                EntityName.ExportFlowOrdersAdvMaterial,
                EntityName.ExportFlowFinancialDataLegalEntity,
                EntityName.ExportFlowCardExtensionsCardCommercial,
                EntityName.PerformedBusinessOperation,
                EntityName.ActionsHistoryDetail,
                EntityName.Building,
                EntityName.DepCard,
                EntityName.UserEntity,
                EntityName.ReleaseValidationResult,
                EntityName.FirmAddressService,
                EntityName.CardRelation,
                EntityName.ReferenceItem,
                EntityName.Reference,
                EntityName.CityPhoneZone,
                EntityName.OrderValidationResult,
                EntityName.ActionsHistory,
                EntityName.ChargesHistory,
                EntityName.Charge,
                EntityName.ReleasesWithdrawalsPosition,
                EntityName.ThemeOrganizationUnit,
                EntityName.ThemeCategory,
                EntityName.OrderPositionAdvertisement,
                EntityName.ReleaseWithdrawal,
                EntityName.OrderReleaseTotal,
                EntityName.UserTerritoriesOrganizationUnits,
                EntityName.TimeZone,
                EntityName.File,
                EntityName.UserOrganizationUnit,
                EntityName.UserTerritory,
                EntityName.CategoryFirmAddress,
                EntityName.CategoryGroupMembership,
                EntityName.KazakhstanLegalPersonProfilePart,
                EntityName.KazakhstanLegalPersonPart,
                EntityName.LegalPersonDeal,
                EntityName.DenormalizedClientLink,
                EntityName.FirmDeal,
                EntityName.ClientLink,
                EntityName.EmiratesFirmAddressPart,
                EntityName.EmiratesBranchOfficeOrganizationUnitPart,
                EntityName.EmiratesLegalPersonProfilePart,
                EntityName.EmiratesClientPart,
                EntityName.EmiratesLegalPersonPart,
                EntityName.UkraineBranchOfficePart,
                EntityName.UkraineLegalPersonPart,
                EntityName.ChileLegalPersonPart,
                EntityName.BusinessEntityPropertyInstance,
                EntityName.BusinessEntityInstance,
                EntityName.DictionaryEntityInstance,
                EntityName.DictionaryEntityPropertyInstance,
                EntityName.OrderProcessingRequestMessage,
                EntityName.MessageType,

                EntityName.LetterRecipient,
                EntityName.LetterSender,
                EntityName.LetterRegardingObject,
                EntityName.TaskRegardingObject,
                EntityName.PhonecallRecipient,
                EntityName.PhonecallRegardingObject,
                EntityName.AppointmentAttendee,
                EntityName.AppointmentRegardingObject,

                EntityName.OrderProcessingRequest,
                EntityName.AdvertisementElementStatus,
                EntityName.AcceptanceReportsJournalRecord,
                EntityName.Bank,
                EntityName.Commune
            };

        private readonly IDictionary<BusinessModel, IEnumerable<EntityName>> _modelSpecificEntities
            = new Dictionary<BusinessModel, IEnumerable<EntityName>>
                  {
                      {
                          BusinessModel.Russia,
                          new[]
                              {
                                  EntityName.OrderProcessingRequest,
                                  EntityName.AdvertisementElementStatus
                              }
                      },
                      {
                          BusinessModel.Chile,
                          new[]
                              {
                                  EntityName.Bank,
                              }
                      },
                      {
                          BusinessModel.Czech,
                          new EntityName[0]
                      },
                      {
                          BusinessModel.Cyprus,
                          new EntityName[0]
                      },
                      {
                          BusinessModel.Ukraine,
                          new EntityName[0]
                      },
                      {
                          BusinessModel.Kazakhstan,
                          new EntityName[0]
                      },
                      {
                          BusinessModel.Emirates,
                          new EntityName[0]
                      },
                  };

        private readonly IUICardConfigurationService _uiConfigurationService;
        private readonly IBusinessModelSettings _businessModelSettings;

        public TestController(IMsCrmSettings msCrmSettings,
                              IUserContext userContext,
                              ICommonLog logger,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IUICardConfigurationService uiConfigurationService,
                              IBusinessModelSettings businessModelSettings)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
        {
            _uiConfigurationService = uiConfigurationService;
            _businessModelSettings = businessModelSettings;
        }

        public EmptyResult GetCardSettings()
        {
            foreach (EntityName entity in Enum.GetValues(typeof(EntityName)))
            {
                if (_entitiesToIgnore.Contains(entity) && !_modelSpecificEntities[_businessModelSettings.BusinessModel].Contains(entity))
                {
                    continue;
                }
             
                try
                {
                    _uiConfigurationService.GetCardSettings(entity, UserContext.Profile.UserLocaleInfo.UserCultureInfo);
                }
                catch (Exception ex)
                {
                    Logger.ErrorEx(ex, entity.ToString());
                }
            }

            return new EmptyResult();
        }
    }
}
