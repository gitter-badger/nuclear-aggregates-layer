﻿using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IUploadFileAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<UpdateIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        UploadFileResult UploadFile(UploadFileParams<TEntity> uploadFileParams);
    }
}