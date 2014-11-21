using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IUploadFileGenericService<TEntity> : IEntityOperation<TEntity>, IUploadFileService 
        where TEntity : class, IEntityKey
    {
    }
}
