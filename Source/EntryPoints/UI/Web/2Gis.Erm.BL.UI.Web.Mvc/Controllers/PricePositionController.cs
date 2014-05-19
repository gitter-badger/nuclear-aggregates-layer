﻿using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class PricePositionController : ControllerBase
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICopyPricePositionOperationService _copyPricePositionOperationService;

        public PricePositionController(IMsCrmSettings msCrmSettings,
                                       IUserContext userContext,
                                       ICommonLog logger,
                                       IAPIOperationsServiceSettings operationsServiceSettings,
                                       IGetBaseCurrencyService getBaseCurrencyService,
                                       IPriceReadModel priceReadModel,
                                       ICopyPricePositionOperationService copyPricePositionOperationService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   getBaseCurrencyService)
        {
            _priceReadModel = priceReadModel;
            _copyPricePositionOperationService = copyPricePositionOperationService;
        }

        [HttpGet]
        public ActionResult Copy(long id)
        {
            var priceId = _priceReadModel.GetPriceId(id);
            return View(new CopyPricePositionModel
                {
                    SourcePricePositionId = id,
                    PriceId = priceId
                });
        }

        [HttpPost]
        public ActionResult Copy(CopyPricePositionModel model)
        {
            try
            {
                _copyPricePositionOperationService.Copy(model.PriceId, model.SourcePricePositionId, model.Position.Key.Value);

                model.Message = BLResources.OK;
            }
            catch (BusinessLogicException ex)
            {
                model.SetCriticalError(ex.Message);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, model, ex);
            }

            return View(model);
        }
    }
}
