using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Charges.ReadModel
{
    public sealed class ChargeReadModel : IChargeReadModel
    {
        private readonly IFinder _finder;

        public ChargeReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public string GetChargesHistoryMessage(Guid sessionId, ChargesHistoryStatus status)
        {
            return _finder.Find<ChargesHistory>(x => x.SessionId == sessionId && x.Status == (int)status).Select(x => x.Message).Single();
        }

        public IReadOnlyCollection<Charge> GetChargesToDelete(long projectId, TimePeriod timePeriod)
        {
            return _finder.Find<Charge>(x => x.ProjectId == projectId && x.PeriodStartDate == timePeriod.Start && x.PeriodEndDate == timePeriod.End).ToArray();
        }

        public bool TryGetLastChargeHistoryId(long projectId, TimePeriod period, ChargesHistoryStatus status, out Guid id)
        {
            id = _finder.Find<ChargesHistory>(x => x.ProjectId == projectId && x.PeriodStartDate == period.Start &&
                                                   x.PeriodEndDate == period.End && x.Status == (int)status)
                        .OrderBy(x => x.CreatedOn)
                        .Select(x => x.SessionId)
                        .FirstOrDefault();

            return id != default(Guid);
        }

        public IReadOnlyCollection<WithdrawalInfoDto> GetBlockingWithdrawals(long destProjectId, TimePeriod period)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<Project>(destProjectId)).Select(x => x.OrganizationUnitId).SingleOrDefault();
            if (organizationUnitId == null)
            {
                return new WithdrawalInfoDto[0];
            }

            var withdrawalInfosQuery = _finder.Find(AccountSpecs.Withdrawals.Find.ForPeriod(period) &&
                                                    AccountSpecs.Withdrawals.Find.InStates(WithdrawalStatus.InProgress,
                                                                                           WithdrawalStatus.Withdrawing,
                                                                                           WithdrawalStatus.Reverting));

            return _finder.Find(Specs.Find.NotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(organizationUnitId.Value, period))
                          .Select(x => x.Order.SourceOrganizationUnit)
                          .GroupJoin(withdrawalInfosQuery,
                                     ou => ou.Id,
                                     wi => wi.OrganizationUnitId,
                                     (ou, wi) => new
                                         {
                                             OrganizationUnit = ou,
                                             LastWithdrawal = wi.OrderByDescending(x => x.StartDate).FirstOrDefault()
                                         })
                          .Where(x => x.LastWithdrawal != null)
                          .Select(x => new WithdrawalInfoDto
            {
                                  OrganizationUnitId = x.OrganizationUnit.Id,
                                  OrganizationUnitName = x.OrganizationUnit.Name,
                                  WithdrawalStatus = x.LastWithdrawal.Status
                              })
                          .ToArray();
        }

        public IReadOnlyDictionary<long, Guid?> GetActualChargesByProject(TimePeriod period)
        {
            var chargesHistoryQuery = _finder.Find<ChargesHistory>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End);

            return _finder.Find(Specs.Find.Active<Project>() && new FindSpecification<Project>(x => x.OrganizationUnitId != null))
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
            var chargesQuery = _finder.Find<Charge>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End);
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId, period))
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