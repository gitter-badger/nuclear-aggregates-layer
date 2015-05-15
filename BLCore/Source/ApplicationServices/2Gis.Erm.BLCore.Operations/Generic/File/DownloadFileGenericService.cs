using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public sealed class DownloadFileGenericService<TEntity> : IDownloadFileGenericService<TEntity> 
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IDownloadFileAggregateRepository<TEntity> _downloadFileService;

        public DownloadFileGenericService(IDownloadFileAggregateRepository<TEntity> downloadFileService)
        {
            _downloadFileService = downloadFileService;
        }
        
        public StreamResponse DownloadFile(long fileId)
        {
            return _downloadFileService.DownloadFile(new DownloadFileParams<TEntity>(fileId));
        }
    }
}
