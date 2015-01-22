using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public class UploadNoteFileService : IUploadFileGenericService<Note>
    {
        private readonly INoteService _noteService;
        private readonly IValidateFileService _validateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public UploadNoteFileService(
            INoteService noteService, 
            IValidateFileService validateService, 
            IOperationScopeFactory scopeFactory)
        {
            _noteService = noteService;
            _validateService = validateService;
            _scopeFactory = scopeFactory;
        }

        public UploadFileResult UploadFile(UploadFileParams uploadFileParams)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UploadIdentity, Note>())
            {
                _validateService.Validate(uploadFileParams);
                var result = _noteService.UploadFile(new UploadFileParams<Note>(uploadFileParams));
                operationScope.Complete();
                return result;
            }
        }
    }
}