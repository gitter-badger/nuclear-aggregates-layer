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
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Ukraine.Controllers
{
    public class LegalPersonController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase, IUkraineAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public LegalPersonController(IMsCrmSettings msCrmSettings,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                     IAPIIdentityServiceSettings identityServiceSettings,
                                     IUserContext userContext,
                                     ICommonLog logger,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                     IPublicService publicService,
                                     ILegalPersonReadModel legalPersonReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
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
            var model = new UkraineChangeLegalPersonRequisitesViewModel
                {
                    Id = legalPerson.Id,
                    Ipn = legalPerson.Inn,
                    LegalAddress = legalPerson.LegalAddress,
                    LegalName = legalPerson.LegalName,
                    LegalPersonType = legalPerson.LegalPersonTypeEnum,
                    Timestamp = legalPerson.Timestamp,
                    TaxationType = legalPerson.Within<UkraineLegalPersonPart>().GetPropertyValue(x => x.TaxationType),
                    LegalPersonP =
                        GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites,
                                                                                     UserContext.Identity.Code))
                };

            model.SetEgrpou(
                legalPerson.Within<UkraineLegalPersonPart>().GetPropertyValue(x => x.Egrpou));

            return View(model);
        }

        [HttpPost, UseDependencyFields, GetEntityStateToken]
        [LogWebRequest(EntityName.LegalPerson, CompareObjectMode = CompareObjectMode.Deep, ElementsToIgnore = "*.Count")]
        public virtual ActionResult ChangeLegalPersonRequisites(UkraineChangeLegalPersonRequisitesViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }
            try
            {
                var request = new UkraineChangeLegalPersonRequisitesRequest
                    {
                        LegalPersonId = model.Id,
                        Ipn = model.Ipn,
                        Egrpou = model.GetEgrpou(),
                        LegalAddress = model.LegalAddress,
                        LegalName = model.LegalName,
                        LegalPersonType = model.LegalPersonType,
                        TaxationType = model.TaxationType
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