using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class PriceController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public PriceController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IPublicService publicService, 
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
        : base(
            msCrmSettings,
            userContext,
            logger,
            operationsServiceSettings,
            getBaseCurrencyService)
        {
            _publicService = publicService;
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
                    var replacePriceRequest = new ReplacePriceRequest
                    {
                        SourcePriceId = viewModel.SourcePriceId,
                        TargetPriceId = viewModel.TargetPrice.Key.Value,
                    };
                    _publicService.Handle(replacePriceRequest);
                    viewModel.Message = BLResources.OK;
                }
                catch (BusinessLogicException ex)
                {
                    viewModel.SetCriticalError(ex.Message);
                }
                catch (Exception ex)
                {
                    ModelUtils.OnException(this, Logger, viewModel, ex);
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
                BeginDate = DateTime.UtcNow.Date.AddMonths(1).AddDays(1 - DateTime.UtcNow.Day)
            });
        }

        [HttpPost]
        public ActionResult CopyNew(CopyNewPriceViewModel viewModel)
        {
            if (!ModelUtils.CheckIsModelValid(this, viewModel))
                return View(viewModel);

            try
            {
                if (!viewModel.OrganizationUnit.Key.HasValue)
                    throw new NotificationException(BLResources.OrganizationUnitNotSet);

                var copyPriceRequest = new CopyPriceRequest
                {
                    SourcePriceId = viewModel.Id,
                    OrganizationUnitId = viewModel.OrganizationUnit.Key.Value,
                    BeginDate = viewModel.BeginDate,
                    PublishDate = viewModel.PublishDate
                };
                _publicService.Handle(copyPriceRequest);

                viewModel.Message = BLResources.OK;
            }
            catch (BusinessLogicException ex)
            {
                viewModel.SetCriticalError(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, viewModel, ex);
            }

            return View(viewModel);
        }
    }
}
