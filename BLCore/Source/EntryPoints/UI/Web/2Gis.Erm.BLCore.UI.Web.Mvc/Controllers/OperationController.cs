using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class OperationController : ControllerBase
    {
        private readonly IOperationService _operationService;

        public OperationController(IMsCrmSettings msCrmSettings,
                                   IAPIOperationsServiceSettings operationsServiceSettings,
                                   IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                   IAPIIdentityServiceSettings identityServiceSettings,
                                   IUserContext userContext,
                                   ITracer tracer,
                                   IGetBaseCurrencyService getBaseCurrencyService,
                                   IOperationService operationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _operationService = operationService;
        }

        [HttpPost]
        public FileResult GetOperationLog(Guid operationId)
        {
            Stream stream;
            var contentType = MediaTypeNames.Text.Plain;
            var fileName = "file.txt";

            if (operationId == Guid.Empty)
            {
                var bytes = Encoding.UTF8.GetBytes(BLResources.IdForOperationNotSpecified);
                stream = new MemoryStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                var operationLogFile = _operationService.GetLogForOperation(operationId);

                if (operationLogFile == null)
                {
                    var bytes = Encoding.UTF8.GetBytes(string.Format(BLResources.OperationWithIdNotFound, operationId));
                    stream = new MemoryStream();
                    stream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                    stream = operationLogFile.Content;
                    contentType = operationLogFile.ContentType;
                    fileName = operationLogFile.FileName;
                }
            }

            return File(stream, contentType, fileName);
        }

        [HttpGet]
        public FileResult OperationLog(Guid operationId)
        {
            return GetOperationLog(operationId);
        }

        [HttpPost]
        public void CreateOperationWithErrorLog(Guid operationId, string log, string contentType, string logFileName)
        {
            var operation = new Operation
                {
                    Guid = operationId,
                    StartTime = DateTime.UtcNow,
                    FinishTime = DateTime.UtcNow,
                    OwnerCode = UserContext.Identity.Code,
                    Status = OperationStatus.Error,
                    Type = BusinessOperation.None,
                };

            _operationService.CreateOperation(operation, log, logFileName);
        }
    }
}
