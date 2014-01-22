using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.PricePositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class PricePositionController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly IFinder _finder;

        public PricePositionController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IPublicService publicService,
            IFinder finder, 
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
            _finder = finder;
        }

        [HttpGet]
        public ActionResult Copy(long id)
        {
            var model = _finder.Find<PricePosition>(x => x.Id == id)
                .Select(x => new CopyPricePositionModel
                                 {
                                     SourcePricePositionId = x.Id,
                                     PriceId = x.PriceId
                                 })
                .Single();
            return View(model);
        }

        [HttpPost]
        public ActionResult Copy(CopyPricePositionModel model)
        {
            try
            {
                _publicService.Handle(new CopyPricePositionRequest
                                  {
                                      SourcePricePositionId = model.SourcePricePositionId,
                                      PositionId = model.Position.Key.Value,
                                      PriceId = model.PriceId
                                  });
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
