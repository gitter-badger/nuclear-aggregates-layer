﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel
{
    public sealed class DealReadModel : IDealReadModel
    {
        private readonly IFinder _finder;

        public DealReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public Deal GetDeal(long id)
        {
            return _finder.Find(Specs.Find.ById<Deal>(id)).SingleOrDefault();
        }

        public Deal GetDeal(Guid replicationCode)
        {
            return _finder.Find(Specs.Find.ByReplicationCode<Deal>(replicationCode)).Single();
        }

        public bool HasOrders(long dealId)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForDeal(dealId) && Specs.Find.ActiveAndNotDeleted<Order>()).Any();
        }

        public AfterSaleServiceActivity GetAfterSaleService(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType serviceType)
        {
            int absoluteMonthNumber = activityDate.ToAbsoluteReleaseNumber();

            return _finder.Find<AfterSaleServiceActivity>(x => x.Deal.ReplicationCode == dealReplicationCode &&
                                                               x.AfterSaleServiceType == (byte)serviceType && x.AbsoluteMonthNumber == absoluteMonthNumber).FirstOrDefault();
        }

        public IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds)
        {
            var result = _finder
                    .Find(Specs.Find.ByIds<Deal>(dealIds))
                    .Select(deal => new DealActualizeDuringWithdrawalDto
                        {
                            Deal = deal,
                            HasInactiveLocks = deal.Orders.Any(o => o.Locks.Any(l => !l.IsActive && !l.IsDeleted))
                        })
                    .ToArray();
            return result;
        }
    }
}