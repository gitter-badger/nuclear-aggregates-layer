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
using DoubleGis.Erm.BLFlex.API.Operations.Global.Kazakhstan.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan;
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
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Kazakhstan.Controllers
{
    public class LegalPersonController : ControllerBase, IKazakhstanAdapted
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
        [HttpGet]
        [UseDependencyFields]
        [SetEntityStateToken]
        public ActionResult ChangeLegalPersonRequisites(long id)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.LegalPersonChangeRequisites, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            var legalPerson = _legalPersonReadModel.GetLegalPerson(id);

            // TODO {01.02.2013}: Убрать получение DTO-объекта в агрегирующий репозиторий
            var model = new KazakhstanChangeLegalPersonRequisitesViewModel
                            {
                                Id = legalPerson.Id,
                                LegalAddress = legalPerson.LegalAddress,
                                LegalName = legalPerson.LegalName,
                                LegalPersonType = (LegalPersonType)legalPerson.LegalPersonTypeEnum,
                                Timestamp = legalPerson.Timestamp,
                                LegalPersonP =
                                    GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites,
                                                                                                 UserContext.Identity.Code))
                            };

            switch (model.LegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    model.Bin = legalPerson.Inn;
                    model.LegalAddress = legalPerson.LegalAddress;
                    break;
                case LegalPersonType.Businessman:
                    model.BinIin = legalPerson.Inn;
                    model.LegalAddress = legalPerson.LegalAddress;
                    break;
                case LegalPersonType.NaturalPerson:
                    model.Iin = legalPerson.Inn;
                    model.IdentityCardNumber = legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(x => x.IdentityCardNumber);
                    model.IdentityCardIssuedOn = legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(x => x.IdentityCardIssuedOn);
                    model.IdentityCardIssuedBy = legalPerson.Within<KazakhstanLegalPersonPart>().GetPropertyValue(x => x.IdentityCardIssuedBy);
                    break;
            }

            return View(model);
        }

        [HttpPost]
        [UseDependencyFields]
        [GetEntityStateToken]
        [LogWebRequest(EntityName.LegalPerson, CompareObjectMode = CompareObjectMode.Deep, ElementsToIgnore = "*.Count")]
        public virtual ActionResult ChangeLegalPersonRequisites(KazakhstanChangeLegalPersonRequisitesViewModel model)
        {
            if (!ModelUtils.CheckIsModelValid(this, model))
            {
                return View(model);
            }
            try
            {
                var request = new KazakhstanChangeLegalPersonRequisitesRequest
                                  {
                                      LegalPersonId = model.Id,
                                      Bin = ReadBin(model.LegalPersonType, model.Bin, model.BinIin, model.Iin),
                                      LegalAddress = model.LegalAddress,
                                      LegalName = model.LegalName,
                                      LegalPersonType = model.LegalPersonType,
                                      IdentityCardNumber = model.IdentityCardNumber,
                                      IdentityCardIssuedBy = model.IdentityCardIssuedBy,
                                      IdentityCardIssuedOn = model.IdentityCardIssuedOn
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
            return new JsonNetResult(
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

        private static string ReadBin(LegalPersonType legalPersonType, string forLegalPerson, string forBusinessman, string forNaturalPerson)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return forLegalPerson;
                case LegalPersonType.Businessman:
                    return forBusinessman;
                case LegalPersonType.NaturalPerson:
                    return forNaturalPerson;
                default:
                    throw new ArgumentException("legalPersonType");
            }
        }
    }
}