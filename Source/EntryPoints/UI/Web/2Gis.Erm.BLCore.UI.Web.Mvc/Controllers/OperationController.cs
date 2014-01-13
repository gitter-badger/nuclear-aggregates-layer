using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class OperationController : ControllerBase
    {
        private readonly IOperationService _operationService;

        public OperationController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IOperationService operationService,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
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

        [HttpPost]
        public void CreateOperationWithErrorLog(Guid operationId, String log, String contentType, String logFileName)
        {
            var operation = new Operation
                {
                    Guid = operationId,
                    StartTime = DateTime.UtcNow,
                    FinishTime = DateTime.UtcNow,
                    OwnerCode = UserContext.Identity.Code,
                    Status = (byte)OperationStatus.Error,
                    Type = (short)BusinessOperation.None,
                };

            _operationService.FinishOperation(operation, log, logFileName);
        }
    }
}
