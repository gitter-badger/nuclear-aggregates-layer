using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Charges.ReadModel
{
    public sealed class ChargeReadModel : IChargeReadModel
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public ChargeReadModel(IQuery query, IFinder finder)
        {
            _query = query;
            _finder = finder;
        }

        public string GetChargesHistoryMessage(Guid sessionId, ChargesHistoryStatus status)
        {
            var p = new MapSpecification<IQueryable<ChargesHistory>, IQueryable<string>>(q => q.Select(x => x.Message));
            return _finder.Find(new FindSpecification<ChargesHistory>(x => x.SessionId == sessionId && x.Status == (int)status))
                          .Map(p)
                          .One();
        }

        public IReadOnlyCollection<Charge> GetChargesToDelete(long projectId, TimePeriod timePeriod)
        {
            return _finder.Find(new FindSpecification<Charge>(x => x.ProjectId == projectId && x.PeriodStartDate == timePeriod.Start && x.PeriodEndDate == timePeriod.End))
                          .Many();
        }

        public bool TryGetLastChargeHistoryId(long projectId, TimePeriod period, ChargesHistoryStatus status, out Guid id)
        {
            id = _finder.Find(new FindSpecification<ChargesHistory>(x => x.ProjectId == projectId && x.PeriodStartDate == period.Start &&
                                                                         x.PeriodEndDate == period.End && x.Status == (int)status))
                        .Map(q => q.OrderBy(x => x.CreatedOn).Select(x => x.SessionId))
                        .Top();

            return id != default(Guid);
        }

        public IReadOnlyDictionary<long, Guid?> GetActualChargesByProject(TimePeriod period)
        {
            var chargesHistoryQuery = _query.For(new FindSpecification<ChargesHistory>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End));
            return _query.For(Specs.Find.Active<Project>() && new FindSpecification<Project>(x => x.OrganizationUnitId != null))
                          .GroupJoin(chargesHistoryQuery, 
                                     p => p.Id, 
                                     ch => ch.ProjectId, 
                                     (p, items) => new
                                         {
                                             ProjectId = p.Id, 
                                             ActualChargeId = items.Where(x => x.Status == (int)ChargesHistoryStatus.Succeeded)
                                                                   .OrderByDescending(x => x.CreatedOn)
                                                                   .Select(x => (Guid?)x.SessionId)
                                                                   .FirstOrDefault()
                                         })
                          .ToDictionary(x => x.ProjectId, x => x.ActualChargeId);
        }

        public IReadOnlyCollection<OrderPositionWithChargeInfoDto> GetPlannedOrderPositionsWithChargesInfo(long organizationUnitId, TimePeriod period)
        {
            var chargesQuery = _query.For(new FindSpecification<Charge>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End));
            return _query.For(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                AccountSpecs.Locks.Find.ForPeriod(period.Start, period.End))
                          .SelectMany(x => x.Order.OrderPositions.Select(op => new { OrderPosition = op, x.Order, Lock = x }))
                          .Where(x => x.OrderPosition.IsActive && !x.OrderPosition.IsDeleted &&
                                        SalesModelUtil.PlannedProvisionSalesModels.Contains(x.OrderPosition.PricePosition.Position.SalesModel))
                          .GroupJoin(chargesQuery, 
                                     opWithlock => opWithlock.OrderPosition.Id, 
                                     charge => charge.OrderPositionId, 
                                     (x, charges) => new OrderPositionWithChargeInfoDto
                                         {
                                             OrderInfo = new OrderInfoDto
                                                 {
                                                     OrderType = x.Order.OrderType, 
                                                     ReleaseCountFact = x.Order.ReleaseCountFact, 
                                                     SourceOrganizationUnitId = x.Order.SourceOrganizationUnitId, 
                                                     DestOrganizationUnitId = x.Order.DestOrganizationUnitId
                                                 }, 
                                             OrderPositionInfo = new OrderPositionInfoDto
                                                 {
                                                     AmountToWithdraw = x.OrderPosition.ReleasesWithdrawals.Where(rw =>
                                                                                                            rw.ReleaseBeginDate == period.Start &&
                                                                                                            rw.ReleaseEndDate == period.End)
                                                                                 .Select(rw => rw.AmountToWithdraw)
                                                                                 .FirstOrDefault(), 
                                                     PriceId = x.OrderPosition.PricePosition.PriceId, 
                                                     CategoryRate = x.OrderPosition.CategoryRate, 
                                                     DiscountPercent = x.OrderPosition.DiscountPercent, 
                                                     OrderPositionId = x.OrderPosition.Id
                                                 }, 
                                             Lock = x.Lock, 
                                             ChargeInfo = charges.Select(c => new ChargeInfoDto
                                                 {
                                                     Amount = c.Amount, 
                                                     ProjectId = c.ProjectId, 
                                                     SessionId = c.SessionId
                                                 }).FirstOrDefault()
                                         })
                          .ToArray();
        }
    }
}