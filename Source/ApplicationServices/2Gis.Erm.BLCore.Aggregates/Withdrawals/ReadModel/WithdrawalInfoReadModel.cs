using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals.ReadModel
{
    public class WithdrawalInfoReadModel : IWithdrawalInfoReadModel
    {
        private readonly IFinder _finder;

        public WithdrawalInfoReadModel(IFinder finder)
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
                                  WithdrawalStatus = (WithdrawalStatus)x.LastWithdrawal.Status
                              })
                          .ToArray();
        }
    }
}