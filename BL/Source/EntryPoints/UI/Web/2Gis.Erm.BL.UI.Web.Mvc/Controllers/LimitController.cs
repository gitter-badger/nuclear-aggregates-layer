using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class LimitController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly ISecureFinder _secureFinder;
        private readonly IRecalculateLimitOperationService _recalculateLimitOperationService;
        private readonly ISetLimitStatusOperationService _setLimitStatusOperationService;
        private readonly ICalculateLimitIncreasingOperationService _calculateLimitIncreasingOperationService;
        private readonly IIncreaseLimitOperationService _increaseLimitOperationService;

        public LimitController(IMsCrmSettings msCrmSettings,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IAPIIdentityServiceSettings identityServiceSettings,
                               IUserContext userContext,
                               ITracer tracer,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               IPublicService publicService,
                               ISecureFinder secureFinder,
                               IRecalculateLimitOperationService recalculateLimitOperationService,
                               ISetLimitStatusOperationService setLimitStatusOperationService,
                               ICalculateLimitIncreasingOperationService calculateLimitIncreasingOperationService,
                               IIncreaseLimitOperationService increaseLimitOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _publicService = publicService;
            _secureFinder = secureFinder;
            _recalculateLimitOperationService = recalculateLimitOperationService;
            _setLimitStatusOperationService = setLimitStatusOperationService;
            _calculateLimitIncreasingOperationService = calculateLimitIncreasingOperationService;
            _increaseLimitOperationService = increaseLimitOperationService;
        }

        #region set status

        [HttpGet]
        public ActionResult SetStatus()
        {
            return View(new SetLimitStatusViewModel());
        }

        [HttpPost]
        public ActionResult SetStatus(SetLimitStatusViewModel model)
        {
            try
            {
                _setLimitStatusOperationService.SetStatus(model.Id, model.Status, model.CrmIds);
                model.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                model.SetCriticalError(ex.Message);
            }

            return View("SetStatus", model);
        }

        [HttpGet]
        public ActionResult CrmSetStatus(Guid[] crmIds, LimitStatus status)
        {
            return View("SetStatus", new SetLimitStatusViewModel { CrmIds = crmIds, Status = status });
        }

        [HttpPost]
        public ActionResult CrmSetStatus(SetLimitStatusViewModel model)
        {
            return SetStatus(model);
        }

        #endregion

        #region print limits

        [HttpGet]
        public ActionResult PrintLimits(string limits)
        {
            const char Delimiter = ',';
            var model = new PrintLimitsViewModel();

            if (string.IsNullOrEmpty(limits.Trim(Delimiter)))
            {
                model.SetWarning(BLResources.PrintLimits_NoLimitsSet);
            }
            else
            {
                try
                {
                    var limitCodes = limits.Split(Delimiter).Select(x => new Guid(x)).ToList();

                    var limitIds = _secureFinder.Find<Limit>(x => limitCodes.Contains(x.ReplicationCode)).Select(x => x.Id).ToArray();
                    model.LimitIds = string.Join(Delimiter.ToString(CultureInfo.InvariantCulture), limitIds);

                    model.LimitCount = limitIds.Length;
                }
                catch (Exception ex)
                {
                    model.SetCriticalError(ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult PrintLimits(PrintLimitsViewModel viewModel)
        {
            try
            {
                var response = (StreamResponse)_publicService.Handle(new PrintLimitsRequest
                {
                    LimitIds = viewModel.LimitIds.Split(',').Select(long.Parse).ToArray(),
                });
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (Exception ex)
            {
                viewModel.SetCriticalError(ex.Message);
                return View(viewModel);
            }
        }

        #endregion

        [HttpPost]
        public EmptyResult Recalculate(long id)
        {
            _recalculateLimitOperationService.Recalculate(id);
            return new EmptyResult();
        }

        [HttpGet]
        public JsonNetResult IncreaseLimit(long limitId)
        {
            decimal amountToIncrease;
            var isLimitIncreasingRequired = _calculateLimitIncreasingOperationService.IsIncreasingRequired(limitId, out amountToIncrease);
            return new JsonNetResult(new
                                         {
                                             IsLimitIncreasingRequired = isLimitIncreasingRequired,
                                             AmountToIncrease = amountToIncrease
                                         });
        }

        [HttpPost]
        public EmptyResult IncreaseLimit(long limitId, decimal amountToIncrease)
        {
            _increaseLimitOperationService.IncreaseLimit(limitId, amountToIncrease);
            return new EmptyResult();
        }
    }
}