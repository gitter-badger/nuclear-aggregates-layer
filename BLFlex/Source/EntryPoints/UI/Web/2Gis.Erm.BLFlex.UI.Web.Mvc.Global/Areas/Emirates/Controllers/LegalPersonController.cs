using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Emirates.Controllers
{
    public class LegalPersonController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase, IEmiratesAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public LegalPersonController(IMsCrmSettings msCrmSettings,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                     IAPIIdentityServiceSettings identityServiceSettings,
                                     IUserContext userContext,
                                     ITracer tracer,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                     IPublicService publicService,
                                     ILegalPersonReadModel legalPersonReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _legalPersonReadModel = legalPersonReadModel;
        }

        // TODO {all, 31.07.2013}: Избавиться от этого костыля
        [HttpGet, UseDependencyFields, SetEntityStateToken]
        public ActionResult ChangeLegalPersonRequisites(long id)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            var legalPerson = _legalPersonReadModel.GetLegalPerson(id);

            // TODO {01.02.2013}: Убрать получение DTO-объекта в агрегирующий репозиторий
            var model = new EmiratesChangeLegalPersonRequisitesViewModel
            {
                Id = legalPerson.Id,
                CommercialLicense = legalPerson.Inn,
                CommercialLicenseBeginDate = legalPerson.Within<EmiratesLegalPersonPart>().GetPropertyValue(x => x.CommercialLicenseBeginDate),
                CommercialLicenseEndDate = legalPerson.Within<EmiratesLegalPersonPart>().GetPropertyValue(x => x.CommercialLicenseEndDate),
                LegalAddress = legalPerson.LegalAddress,
                LegalName = legalPerson.LegalName,
                Timestamp = legalPerson.Timestamp,
                LegalPersonP =
                    GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites,
                                                                                 UserContext.Identity.Code))
            };

            return View(model);
        }

        [HttpPost, UseDependencyFields, GetEntityStateToken]
        [LogWebRequest("LegalPerson", CompareObjectMode = CompareObjectMode.Deep, ElementsToIgnore = "*.Count")]
        public virtual ActionResult ChangeLegalPersonRequisites(EmiratesChangeLegalPersonRequisitesViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }
            try
            {
                var request = new EmiratesChangeLegalPersonRequisitesRequest
                {
                    LegalPersonId = model.Id,
                    CommercialLicense = model.CommercialLicense,
                    CommercialLicenseBeginDate = model.CommercialLicenseBeginDate.Value,
                    CommercialLicenseEndDate = model.CommercialLicenseEndDate.Value,
                    LegalAddress = model.LegalAddress,
                    LegalName = model.LegalName
                };

                _publicService.Handle(request);
                model.Message = BLResources.OK;
            }
            catch (NotificationException ex)
            {
                model.SetCriticalError(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Tracer, model, ex);
            }

            return View(model);
        }

        public JsonNetResult GetPaymentMethod(long legalPersonId)
        {
            return
                new JsonNetResult(
                    new
                        {
                            PaymentMethod = _legalPersonReadModel.GetPaymentMethod(legalPersonId)
                        });
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