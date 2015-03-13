﻿using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class PriceController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly ICopyPriceOperationService _copyPriceOperationService;
        private readonly IReplacePriceOperationService _replacePriceOperationService;

        public PriceController(IMsCrmSettings msCrmSettings,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IIdentityServiceClientSettings identityServiceSettings,
                               IUserContext userContext,
                               ITracer tracer,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               IPublicService publicService,
                               ICopyPriceOperationService copyPriceOperationService,
                               IReplacePriceOperationService replacePriceOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _publicService = publicService;
            _copyPriceOperationService = copyPriceOperationService;
            _replacePriceOperationService = replacePriceOperationService;
        }

        [HttpGet]
        public ActionResult Publish()
        {
            return View(new PublishPriceViewModel());
        }

        [HttpPost]
        public ActionResult Publish(PublishPriceViewModel viewModel)
        {
            try
            {
                var publishPriceRequest = new PublishPriceRequest
                                              {
                                                  PriceId = viewModel.Id,
                                                  OrganizarionUnitId = viewModel.OrganizationUnitId,
                                                  BeginDate = viewModel.BeginDate,
                                                  PublishDate = viewModel.PublishDate
                                              };
                _publicService.Handle(publishPriceRequest);

                viewModel.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                viewModel.SetCriticalError(ex.Message);
                return View(viewModel);
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Unpublish()
        {
            return View(new UnpublishPriceViewModel());
        }

        [HttpPost]
        public ActionResult Unpublish(UnpublishPriceViewModel viewModel)
        {
            try
            {
                var unpublishPriceRequest = new UnpublishPriceRequest
                {
                    PriceId = viewModel.PriceId,
                };
                _publicService.Handle(unpublishPriceRequest);

                viewModel.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                viewModel.SetCriticalError(ex.Message);
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Copy()
        {
            return View(new CopyPriceViewModel { CopyNewPrice = true });
        }

        [HttpPost]
        public ActionResult Copy(CopyPriceViewModel viewModel)
        {
            if (ModelUtils.CheckIsModelValid(this, viewModel))
            {
                try
                {
                    _replacePriceOperationService.Replace(viewModel.SourcePriceId, viewModel.TargetPrice.Key.Value);

                    viewModel.Message = BLResources.OK;
                }
                catch (BusinessLogicException ex)
                {
                    viewModel.SetCriticalError(ex.Message);
                }
                catch (Exception ex)
                {
                    ModelUtils.OnException(this, Tracer, viewModel, ex);
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult CopyNew()
        {
            return View(new CopyNewPriceViewModel
            {
                PublishDate = DateTime.UtcNow.Date.AddDays(1),
                BeginDate = DateTime.UtcNow.Date.GetNextMonthFirstDate()
            });
        }

        [HttpPost]
        public ActionResult CopyNew(CopyNewPriceViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
            {
                return View(viewModel);
            }

            try
            {
                if (!viewModel.OrganizationUnit.Key.HasValue)
                {
                    throw new NotificationException(BLResources.OrganizationUnitNotSet);
                }

                _copyPriceOperationService.Copy(viewModel.Id, viewModel.OrganizationUnit.Key.Value, viewModel.PublishDate, viewModel.BeginDate);

                viewModel.Message = BLResources.OK;
            }
            catch (BusinessLogicException ex)
            {
                viewModel.SetCriticalError(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Tracer, viewModel, ex);
            }

            return View(viewModel);
        }
    }
}
