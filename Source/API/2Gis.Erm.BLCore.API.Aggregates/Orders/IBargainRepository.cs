using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders
{
    public interface IBargainRepository : IAggregateRootRepository<Bargain>,
                                          IDeleteAggregateRepository<Bargain>,
                                          IDownloadFileAggregateRepository<BargainFile>,
                                          IUploadFileAggregateRepository<BargainFile>
    {
        int Delete(Bargain entity);
        BargainUsageDto GetBargainUsage(long entityId);
        int Update(Bargain bargain);

        IReadOnlyCollection<Bargain> GetNonClosedBargains();
        IReadOnlyCollection<Bargain> GetBargainsForOrder(long? legalPersonId, long? branchOfficeOrganizationUnitId);

        void CloseBargains(IEnumerable<Bargain> bargains, DateTime closeDate);
        void CreateOrUpdate(BargainFile entity);
    }
}