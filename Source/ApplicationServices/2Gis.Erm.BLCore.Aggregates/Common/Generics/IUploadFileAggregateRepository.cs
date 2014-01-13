using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface IUploadFileAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<UpdateIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        UploadFileResult UploadFile(UploadFileParams<TEntity> uploadFileParams);
    }
}
