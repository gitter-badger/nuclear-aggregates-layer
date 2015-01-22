using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public sealed class UploadFileGenericService<TEntity> : IUploadFileGenericService<TEntity> 
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IUploadFileAggregateRepository<TEntity> _uploadFileAggregateRepository;
        private readonly IValidateFileService _validateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public UploadFileGenericService(
            IUploadFileAggregateRepository<TEntity> uploadFileAggregateRepository, 
            IValidateFileService validateService, 
            IOperationScopeFactory scopeFactory)
        {
            _uploadFileAggregateRepository = uploadFileAggregateRepository;
            _validateService = validateService;
            _scopeFactory = scopeFactory;
        }

        public UploadFileResult UploadFile(UploadFileParams uploadFileParams)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UploadIdentity, TEntity>())
            {
                _validateService.Validate(uploadFileParams);
                var result = _uploadFileAggregateRepository.UploadFile(new UploadFileParams<TEntity>(uploadFileParams));

                operationScope
                    .Updated<DoubleGis.Erm.Platform.Model.Entities.Erm.File>(result.FileId)
                    .Complete(); 

                return result;
            }
        }
    }
}
