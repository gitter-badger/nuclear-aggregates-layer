using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IDownloadFileGenericService<TEntity> : IEntityOperation<TEntity>, IDownloadFileService 
        where TEntity : class, IEntityKey
    {
    }
}
