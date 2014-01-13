using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders
{
    public interface IBargainRepository : IAggregateRootRepository<Bargain>,
                                          IDeleteAggregateRepository<Bargain>,
                                          IDownloadFileAggregateRepository<BargainFile>,
                                          IUploadFileAggregateRepository<BargainFile>
    {
        int Delete(Bargain entity);
        Bargain Find(long entityId);
        BargainUsageDto GetBargainUsage(long entityId);
        int Update(Bargain bargain);
        IEnumerable<Bargain> FindBySpecification(IFindSpecification<Bargain> spec);
        void CloseBargains(IEnumerable<Bargain> bargains, DateTime closeDate);

        IQueryable<TOutput> FindBySpecification<TOutput>(ISelectSpecification<Bargain, TOutput> selectSpecification, IFindSpecification<Bargain> spec);

        long GenerateNextBargainUniqueNumber();
        void CreateOrUpdate(Bargain bargain);
        void CreateOrUpdate(BargainFile entity);
    }
}