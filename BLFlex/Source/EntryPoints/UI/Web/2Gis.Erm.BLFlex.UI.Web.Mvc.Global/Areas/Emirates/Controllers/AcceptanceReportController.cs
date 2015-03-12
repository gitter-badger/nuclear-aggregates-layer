using System;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Emirates.Controllers
{
    public class AcceptanceReportController : BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase
    {
        private readonly IPublicService _publicService;

        public AcceptanceReportController(IMsCrmSettings msCrmSettings,
                                          IAPIOperationsServiceSettings operationsServiceSettings,
                                          IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                          IIdentityServiceClientSettings identityServiceSettings,
                                          IUserContext userContext,
                                          ITracer tracer,
                                          IGetBaseCurrencyService getBaseCurrencyService,
                                          IPublicService publicService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _publicService = publicService;
        }

        [HttpGet]
        public ActionResult Generate()
        {
            var currentUser = UserContext.Identity;

            var model = new EmiratesGenerateAcceptanceReportViewModel
                {
                    Month = DateTime.Today.GetFirstDateOfMonth(),
                    UserId = currentUser.Code
                };

            return View(model);
        }

        [HttpGet]
        public ActionResult Print(long organizationUnitId, DateTime month)
        {
            var request = new EmiratesPrintAcceptanceReportsRequest
                {
                    Month = month.GetEndPeriodOfThisMonth(),
                    OrganizationUnitId = organizationUnitId
                };

            try
            {
                var response = (StreamResponse)_publicService.Handle(request);
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (AcceptanceReportPrintingException ex)
            {
                return new ContentResult { Content = ex.Message };
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = ex.Message };
            }
        }
    }
}