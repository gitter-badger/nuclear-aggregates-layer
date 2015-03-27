using System;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Russia.Controllers
{
    public sealed class PrintController : ControllerBase
    {
        private readonly IPrintBindingChangeAgreementOperationService _printBindingChangeAgreementService;
        private readonly IPrintFirmChangeAgreementOperationService _printFirmChangeAgreementService;
        private readonly IPrintCancellationAgreementOperationService _printCancellationAgreementService;

        public PrintController(IMsCrmSettings crmSettings, 
                               IAPIOperationsServiceSettings operationsServiceSettings, 
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings, 
                               IAPIIdentityServiceSettings identityServiceSettings, 
                               IUserContext userContext, 
                               ITracer tracer, 
                               IGetBaseCurrencyService getBaseCurrencyService, 
                               IPrintFirmChangeAgreementOperationService printFirmChangeAgreementService, 
                               IPrintCancellationAgreementOperationService printCancellationAgreementService, 
                               IPrintBindingChangeAgreementOperationService printBindingChangeAgreementService)
            : base(crmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _printFirmChangeAgreementService = printFirmChangeAgreementService;
            _printCancellationAgreementService = printCancellationAgreementService;
            _printBindingChangeAgreementService = printBindingChangeAgreementService;
        }

        [HttpGet]
        public ActionResult BindingChangeAgreement(long id)
        {
            return ExecutePrint(() => _printBindingChangeAgreementService.Print(id));
        }

        [HttpGet]
        public ActionResult FirmChangeAgreement(long id)
        {
            return ExecutePrint(() => _printFirmChangeAgreementService.Print(id));
        }

        [HttpGet]
        public ActionResult CancellationAgreement(long id)
        {
            return ExecutePrint(() => _printCancellationAgreementService.Print(id));
        }

        private ActionResult ExecutePrint(Func<PrintFormDocument> print)
        {
            try
            {
                var document = print.Invoke();
                return File(document.Stream, document.ContentType, HttpUtility.UrlPathEncode(document.FileName));
            }
            catch (BusinessLogicException exception)
            {
                return new ContentResult { ContentType = MediaTypeNames.Text.Plain, Content = exception.Message };
            }
        }
    }
}