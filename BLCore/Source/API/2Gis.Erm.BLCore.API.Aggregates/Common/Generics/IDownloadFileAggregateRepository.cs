﻿using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IDownloadFileAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<DownloadIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        StreamResponse DownloadFile(DownloadFileParams<TEntity> downloadFileParams);
    }
}