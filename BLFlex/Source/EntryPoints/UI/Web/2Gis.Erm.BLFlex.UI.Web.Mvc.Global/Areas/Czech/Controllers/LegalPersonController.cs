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
using DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Czech.Controllers
{
    public class LegalPersonController : ControllerBase, ICzechAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;
        private readonly ILegalPersonReadModel _legalPersonReadModel;


        // TODO {all, 31.07.2013}: Избавиться от этого костыля
        public LegalPersonController(IMsCrmSettings msCrmSettings,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                     IIdentityServiceClientSettings identityServiceSettings,
                                     IUserContext userContext,
                                     ITracer tracer,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                     IPublicService publicService,
                                     IFinder finder,
                                     ILegalPersonReadModel legalPersonReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
        }

        [HttpGet, UseDependencyFields, SetEntityStateToken]
        public ActionResult ChangeLegalPersonRequisites(long id)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            // TODO {01.02.2013}: Убрать получение DTO-объекта в агрегирующий репозиторий
            var model = _finder.Find(new FindSpecification<LegalPerson>(x => x.Id == id))
            .Select(legalPerson => new CzechChangeLegalPersonRequisitesViewModel
            {
                Id = legalPerson.Id,
                Inn = legalPerson.Inn,
                Ic = legalPerson.Ic,
                CardNumber = legalPerson.CardNumber,
                LegalAddress = legalPerson.LegalAddress,
                LegalName = legalPerson.LegalName,
                LegalPersonType = legalPerson.LegalPersonTypeEnum,
                Timestamp = legalPerson.Timestamp
            })
            .Single();

            model.LegalPersonP = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code));

            return View(model);
        }

        [HttpPost, UseDependencyFields, GetEntityStateToken]
        [LogWebRequest("LegalPerson", CompareObjectMode = CompareObjectMode.Deep, ElementsToIgnore = "*.Count")]
        public virtual ActionResult ChangeLegalPersonRequisites(CzechChangeLegalPersonRequisitesViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }
            try
            {
                _publicService.Handle(new CzechChangeLegalPersonRequisitesRequest
                {
                    LegalPersonId = model.Id,
                    Inn = model.Inn,
                    Ic = model.Ic,
                    CardNumber = model.CardNumber,
                    LegalAddress = model.LegalAddress,
                    LegalName = model.LegalName,
                    LegalPersonType = model.LegalPersonType,
                });
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