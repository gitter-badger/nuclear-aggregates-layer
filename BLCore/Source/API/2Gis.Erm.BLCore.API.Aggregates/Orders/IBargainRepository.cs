using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders
{
    public interface IBargainRepository : IAggregateRootService<Bargain>,
                                          IDownloadFileAggregateRepository<BargainFile>,
                                          IUploadFileAggregateRepository<BargainFile>
    {
        void CreateOrUpdate(BargainFile entity);
    }
}