﻿using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Chile.Controllers
{
    public class LegalPersonController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase, IChileAdapted
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public LegalPersonController(IMsCrmSettings msCrmSettings,
                                     IUserContext userContext,
                                     ICommonLog logger,
                                     IAPIOperationsServiceSettings operationsServiceSettings,
                                     IGetBaseCurrencyService getBaseCurrencyService,
                                     ISecurityServiceFunctionalAccess functionalAccessService,
                                     IPublicService publicService,
                                     IFinder finder,
                                     ILegalPersonReadModel legalPersonReadModel)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _finder = finder;
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

            // TODO {01.02.2013}: Убрать получение DTO-объекта в агрегирующий репозиторий
            var model = _finder.Find<LegalPerson>(x => x.Id == id)
            .Select(legalPerson => new ChileChangeLegalPersonRequisitesViewModel
            {
                Id = legalPerson.Id,
                Rut = legalPerson.Inn,
                LegalAddress = legalPerson.LegalAddress,
                LegalName = legalPerson.LegalName,
                Timestamp = legalPerson.Timestamp
            })
            .Single();

            var communeReference = _legalPersonReadModel.GetCommuneReference(id);

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
            // TODO {y.baranihin, 20.01.2014}: использовать ReadModel, когда она появится
            return
                new JsonNetResult(
                    new
                    {
                        PaymentMethod =
                    _finder.Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId))
                           .Select(x => (PaymentMethod?)x.PaymentMethod)
                           .SingleOrDefault()
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