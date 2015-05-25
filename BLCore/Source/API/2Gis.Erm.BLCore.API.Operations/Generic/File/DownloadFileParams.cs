using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public sealed class DownloadFileParams<TEntity> where TEntity : class, IEntityKey
    {
        public DownloadFileParams(long fileId)
        {
            FileId = fileId;
        }

        public long FileId { get; private set; }
    }
}
