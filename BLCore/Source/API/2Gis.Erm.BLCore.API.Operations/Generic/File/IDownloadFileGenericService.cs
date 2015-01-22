using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IDownloadFileGenericService<TEntity> : IEntityOperation<TEntity>, IDownloadFileService 
        where TEntity : class, IEntityKey
    {
    }
}
