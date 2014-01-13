using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel
{
    public sealed class DealReadModel : IDealReadModel
    {
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;

        public DealReadModel(IFinder finder, ISecureFinder secureFinder)
        {
            _finder = finder;
            _secureFinder = secureFinder;
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

        public IEnumerable<DealActualizeProfitDto> GetInfoForActualizeProfits(IEnumerable<long> dealIds, bool processActuallyReceivedProfit)
        {
            // FIXME {i.maslennikov, 30.09.2013}: проверить корректность переписанной выборки
            // старая реализация public void RecalcEstimatedProfit(Deal deal)
            //{
            //    var ordersForDeal = _finder.Find(OrderSpecs.Orders.Find.ForDeal(deal.Id) 
            //                                        && OrderSpecs.Orders.Find.NotRejected());
            //    var inactiveLocks = _finder.Find(Specs.Find.InactiveEntities<Lock>() && AccountSpecs.Locks.Find.ForOrders(ordersForDeal.Select(x => x.Id)));

            //    var withdrawInfo = (from order in ordersForDeal
            //                        from releaseTotal in order.OrderReleaseTotals
            //                        where !inactiveLocks.Any(x => x.OrderId == releaseTotal.OrderId &&
            //                                                      x.PeriodStartDate == releaseTotal.ReleaseBeginDate &&
            //                                                      x.PeriodEndDate == releaseTotal.ReleaseEndDate)
            //                        group new { releaseTotal.Id, AmountToWithdraw = (decimal?)releaseTotal.AmountToWithdraw }
            //                            by new
            //                            {
            //                                releaseTotal.OrderId,
            //                                ReleasesCountLeft = (releaseTotal.Order.ReleaseCountPlan > releaseTotal.Order.ReleaseCountFact)
            //                                                        ? releaseTotal.Order.ReleaseCountFact
            //                                                        : releaseTotal.Order.ReleaseCountPlan
            //                            }
            //                            into grouping
            //                            select new
            //                            {
            //                                OrderInfo = grouping.Key,
            //                                Releases = grouping.OrderBy(x => x.Id)
            //                            })
            //        .ToArray();

            //    // беру списания по реальным выпускам (отсекаю выпуски если заказ расторгнут и факт. число выпусков не равно планируемому) 
            //    deal.EstimatedProfit = withdrawInfo.SelectMany(x => x.Releases.Select(y => y.AmountToWithdraw ?? 0m).Take(x.OrderInfo.ReleasesCountLeft)).Sum();
            //    Update(deal);
            //}
            var result = _secureFinder
                    .Find(DealSpecs.Deals.Select.ActualizeProfitInfo(processActuallyReceivedProfit), Specs.Find.ByIds<Deal>(dealIds))
                    .ToArray();

            return result;
        }

        public IEnumerable<DealActualizeDuringWithdrawalDto> GetInfoForWithdrawal(IEnumerable<long> dealIds)
        {
            var result = _finder
                    .Find<Deal, DealActualizeProfitDto>(DealSpecs.Deals.Select.ActualizeProfitInfo(true), Specs.Find.ByIds<Deal>(dealIds))
                    .Select(dto => new DealActualizeDuringWithdrawalDto
                        {
                            ActualizeProfitInfo = dto,
                            HasInactiveLocks = dto.Deal.Orders.Any(o => o.Locks.Any(l => !l.IsActive && !l.IsDeleted))
                        })
                    .ToArray();
            return result;
        }
    }
}