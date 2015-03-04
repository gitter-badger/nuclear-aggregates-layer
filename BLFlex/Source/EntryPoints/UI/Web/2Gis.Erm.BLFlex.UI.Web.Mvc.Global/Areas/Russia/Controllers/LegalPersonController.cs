using System;
using System.Linq;
using System.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Russia.Controllers
{
    public class LegalPersonController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IReplicationCodeConverter _replicationCodeConverter;
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;

        public LegalPersonController(IMsCrmSettings msCrmSettings,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                     IAPIIdentityServiceSettings identityServiceSettings,
                                     IUserContext userContext,
                                     ICommonLog logger,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     IOperationServicesManager operationServicesManager,
                                     ISecurityServiceUserIdentifier userIdentifierService,
                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                     IReplicationCodeConverter replicationCodeConverter,
                                     IPublicService publicService,
                                     IFinder finder)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _replicationCodeConverter = replicationCodeConverter;
            _publicService = publicService;
            _finder = finder;
        }

        #region Merge Legal Persons

        public ActionResult Merge(long masterId, long? subordinateId)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.MergeLegalPersons, UserContext.Identity.Code))
            {
                throw new SecurityException(BLResources.AccessDenied);
            }

            var legalPerson1 = _finder.FindOne(Specs.Find.ById<LegalPerson>(masterId) && Specs.Find.ActiveAndNotDeleted<LegalPerson>());
            var legalPerson2 = subordinateId.HasValue
                                   ? _finder.FindOne(Specs.Find.ById<LegalPerson>(subordinateId.Value) && Specs.Find.ActiveAndNotDeleted<LegalPerson>())
                                   : null;

            var model = new MergeLegalPersonsViewModel
            {
                LegalPerson1 = new LookupField { Key = legalPerson1.Id, Value = legalPerson1.LegalName },
                LegalPerson2 = (legalPerson2 != null) ? new LookupField { Key = legalPerson2.Id, Value = legalPerson2.LegalName } : null
            };

            if (legalPerson2 != null && legalPerson1.LegalPersonTypeEnum != legalPerson2.LegalPersonTypeEnum)
            {
                model.LegalPerson2 = null;
                model.SetCriticalError(BLResources.MergeLegalPersonsDifferentTypesError);
            }
            else if (legalPerson2 != null && legalPerson1.LegalPersonTypeEnum == LegalPersonType.LegalPerson
                && (legalPerson1.Inn != legalPerson2.Inn || legalPerson1.Kpp != legalPerson2.Kpp))
            {
                model.LegalPerson2 = null;
                model.SetCriticalError(BLResources.MergeLegalPersonsDifferentKPPINNError);
            }
            else if (legalPerson2 != null && legalPerson1.LegalPersonTypeEnum == LegalPersonType.Businessman
                && (legalPerson1.Inn != legalPerson2.Inn))
            {
                model.LegalPerson2 = null;
                model.SetCriticalError(BLResources.MergeLegalPersonsDifferentINNError);
            }
            else if (legalPerson2 != null && legalPerson1.LegalPersonTypeEnum == LegalPersonType.NaturalPerson
                && (legalPerson1.PassportNumber != legalPerson2.PassportNumber || legalPerson1.PassportSeries != legalPerson2.PassportSeries))
            {
                model.LegalPerson2 = null;
                model.SetCriticalError(BLResources.MergeLegalPersonsDifferentPassportError);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult CrmMerge(Guid[] crmIds)
        {
            if (!(crmIds.Length == 1 || crmIds.Length == 2))
            {
                throw new NotificationException("Для операции слияния нужно выбрать один или два элемента");
            }

            var masterId = _replicationCodeConverter.ConvertToEntityId(EntityName.LegalPerson, crmIds[0]);
            var subordinateId = (long?)null;

            if (crmIds.Length == 2)
            {
                subordinateId = _replicationCodeConverter.ConvertToEntityId(EntityName.LegalPerson, crmIds[1]);
            }

            return RedirectToAction("Merge", "LegalPerson", new { masterId, subordinateId });
        }

        [HttpPost]
        public ActionResult Merge(MergeLegalPersonsViewModel model)
        {
            try
            {
                if (!model.AppendedLegalPersonId.HasValue)
                {
                    throw new NotificationException("Не выбрана дополнительная запись");
                }

                if (!model.MainLegalPersonId.HasValue)
                {
                    throw new NotificationException("Не выбрана главная запись");
                }

                _publicService.Handle(new MergeLegalPersonsRequest
                {
                    AppendedLegalPersonId = model.AppendedLegalPersonId.Value,
                    MainLegalPersonId = model.MainLegalPersonId.Value
                });
                model.Message = BLResources.OK;
            }
            catch (NotificationException ex)
            {
                model.SetCriticalError(ex.Message);
            }

            return View(model);
        }

        public ActionResult MergeLegalPersonsGetData(long masterId, long subordinateId)
        {
            var service = _operationServicesManager.GetDomainEntityDtoService(EntityName.LegalPerson);
            var masterLegalPersonDto = service.GetDomainEntityDto(masterId, false, null, EntityName.None, string.Empty);
            var subordinateLegalPersonDto = service.GetDomainEntityDto(subordinateId, false, null, EntityName.None, string.Empty);

            var masterLegalPersonModel = new LegalPersonViewModel();
            masterLegalPersonModel.LoadDomainEntityDto(masterLegalPersonDto);

            var subordinateIdLegalPersonModel = new LegalPersonViewModel();
            subordinateIdLegalPersonModel.LoadDomainEntityDto(subordinateLegalPersonDto);

            var model = new MergeLegalPersonsDataViewModel
            {
                    LegalPerson1 = masterLegalPersonModel,
                    LegalPerson2 = subordinateIdLegalPersonModel,
            };

            model.LegalPerson1.Owner.Value = _userIdentifierService.GetUserInfo(model.LegalPerson1.Owner.Key).DisplayName;
            model.LegalPerson2.Owner.Value = _userIdentifierService.GetUserInfo(model.LegalPerson2.Owner.Key).DisplayName;
            model.LegalPerson1.SetEntityStateToken();
            model.LegalPerson2.SetEntityStateToken();

            return View(model);
        }

        #endregion

        [HttpGet, UseDependencyFields, SetEntityStateToken]
        public ActionResult ChangeLegalPersonRequisites(long id)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            // TODO {01.02.2013}: Убрать получение DTO-объекта в агрегирующий репозиторий
            var model = _finder.Find<LegalPerson>(x => x.Id == id)
            .Select(x => new ChangeLegalPersonRequisitesViewModel
            {
                Id = x.Id,
                Inn = x.LegalPersonTypeEnum == LegalPersonType.LegalPerson ? x.Inn : null,
                BusinessmanInn = x.LegalPersonTypeEnum == LegalPersonType.Businessman ? x.Inn : null,
                Kpp = x.Kpp,
                LegalAddress = x.LegalAddress,
                LegalName = x.LegalName,
                LegalPersonType = x.LegalPersonTypeEnum,
                PassportSeries = x.PassportSeries,
                PassportNumber = x.PassportNumber,
                RegistrationAddress = x.RegistrationAddress,
                ShortName = x.ShortName,
                Timestamp = x.Timestamp
            })
            .Single();

            model.LegalPersonP = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code));
            
            return View(model);
        }

        [HttpPost, UseDependencyFields, GetEntityStateToken]
        [LogWebRequest(EntityName.LegalPerson, CompareObjectMode = CompareObjectMode.Deep, ElementsToIgnore = "*.Count")]
        public virtual ActionResult ChangeLegalPersonRequisites(ChangeLegalPersonRequisitesViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }

            try
            {
                _publicService.Handle(new ChangeLegalPersonRequisitesRequest
                                  {
                                      LegalPersonId = model.Id,
                                      Inn = model.LegalPersonType == LegalPersonType.Businessman ? model.BusinessmanInn : model.Inn,
                                      Kpp = model.Kpp,
                                      LegalAddress = model.LegalAddress,
                                      LegalName = model.LegalName,
                                      LegalPersonType = model.LegalPersonType,
                                      PassportSeries = model.PassportSeries,
                                      PassportNumber = model.PassportNumber,
                                      RegistrationAddress = model.RegistrationAddress,
                    ShortName = model.ShortName,
                                  });

                model.Message = BLResources.OK;
            }
            catch (NotificationException ex)
            {
                model.SetCriticalError(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, model, ex);
            }

            return View(model);
        }

        private static LegalPersonChangeRequisitesAccess GetMaxAccess(int[] accesses)
        {
            if (!accesses.Any())
            {
                return LegalPersonChangeRequisitesAccess.None;
            }

            var priorities = new[] { LegalPersonChangeRequisitesAccess.None, LegalPersonChangeRequisitesAccess.GrantedLimited, LegalPersonChangeRequisitesAccess.Granted };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (LegalPersonChangeRequisitesAccess)x)).Max();
            return priorities[maxPriority];
        }
    }
}
