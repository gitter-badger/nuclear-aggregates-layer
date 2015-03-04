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
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Chile.Controllers
{
    public class LegalPersonController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase, IChileAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IChileLegalPersonReadModel _chileLegalPersonReadModel;

         

        // TODO {all, 31.07.2013}: Избавиться от этого костыля
        public LegalPersonController(IMsCrmSettings msCrmSettings,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                     IAPIIdentityServiceSettings identityServiceSettings,
                                     IUserContext userContext,
                                     ITracer logger,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                     IPublicService publicService,
                                     ILegalPersonReadModel legalPersonReadModel,
                                     IChileLegalPersonReadModel chileLegalPersonReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _legalPersonReadModel = legalPersonReadModel;
            _chileLegalPersonReadModel = chileLegalPersonReadModel;
        }

        [HttpGet, UseDependencyFields, SetEntityStateToken]
        public ActionResult ChangeLegalPersonRequisites(long id)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            // TODO {01.02.2013}: Убрать получение DTO-объекта в агрегирующий репозиторий
            var legalPerson = _legalPersonReadModel.GetLegalPerson(id);

            var model =
                new ChileChangeLegalPersonRequisitesViewModel
                    {
                        Id = legalPerson.Id,
                        OperationsKind = legalPerson.Within<ChileLegalPersonPart>().GetPropertyValue(x => x.OperationsKind),
                        Rut = legalPerson.Inn,
                        LegalAddress = legalPerson.LegalAddress,
                        LegalName = legalPerson.LegalName,
                        Timestamp = legalPerson.Timestamp
                    };

            var communeReference = _chileLegalPersonReadModel.GetCommuneReference(id);

            model.Commune = LookupField.FromReference(communeReference);

            model.LegalPersonP = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code));

            return View(model);
        }

        [HttpPost, UseDependencyFields, GetEntityStateToken]
        [LogWebRequest(EntityName.LegalPerson, CompareObjectMode = CompareObjectMode.Deep, ElementsToIgnore = "*.Count")]
        public virtual ActionResult ChangeLegalPersonRequisites(ChileChangeLegalPersonRequisitesViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }
            try
            {
                _publicService.Handle(new ChileChangeLegalPersonRequisitesRequest
                {
                    LegalPersonId = model.Id,
                    Rut = model.Rut,
                    LegalAddress = model.LegalAddress,
                    LegalName = model.LegalName,
                    OperationsKind = model.OperationsKind,
                    CommuneId = model.Commune.Key.Value
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