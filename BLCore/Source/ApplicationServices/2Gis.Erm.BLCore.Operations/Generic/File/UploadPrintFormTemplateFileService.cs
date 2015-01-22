using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public class UploadPrintFormTemplateFileService : IUploadFileGenericService<PrintFormTemplate>
    {
        private readonly IPrintFormTemplateService _printFormTemplateService;
        private readonly IValidateFileService _validateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public UploadPrintFormTemplateFileService(
            IPrintFormTemplateService printFormTemplateService,
            IValidateFileService validateService,
            IOperationScopeFactory scopeFactory)
        {
            _printFormTemplateService = printFormTemplateService;
            _validateService = validateService;
            _scopeFactory = scopeFactory;
        }

        public UploadFileResult UploadFile(UploadFileParams uploadFileParams)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UploadIdentity, PrintFormTemplate>())
            {
                _validateService.Validate(uploadFileParams);
                var result = _printFormTemplateService.UploadPrintFormTemplateFile(new UploadFileParams<PrintFormTemplate>(uploadFileParams));
                operationScope.Complete();
                return result;
            }
        }
    }
}
