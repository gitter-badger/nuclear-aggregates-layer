﻿using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders
{
    public interface IBargainRepository : IAggregateRootRepository<Bargain>,
                                          IDownloadFileAggregateRepository<BargainFile>,
                                          IUploadFileAggregateRepository<BargainFile>
    {
        void CreateOrUpdate(BargainFile entity);
    }
}