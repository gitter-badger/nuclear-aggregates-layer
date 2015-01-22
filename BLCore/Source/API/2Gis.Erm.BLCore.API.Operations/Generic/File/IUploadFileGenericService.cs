using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    public interface IUploadFileGenericService<TEntity> : IEntityOperation<TEntity>, IUploadFileService 
        where TEntity : class, IEntityKey
    {
    }
}
