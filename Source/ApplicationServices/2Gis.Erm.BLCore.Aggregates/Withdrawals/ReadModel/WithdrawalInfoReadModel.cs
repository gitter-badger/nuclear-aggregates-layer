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

        public bool CanCreateCharges(long projectId, TimePeriod timePeriod, out string error)
        {
            // FIXME {all, 23.05.2014}: Проверка отключена, требования уточняются
            error = null;
            return true;

            var organizationUnitId = _finder.Find(Specs.Find.ById<Project>(projectId)).Select(x => x.OrganizationUnitId).SingleOrDefault();
            if (organizationUnitId == null)
            {
                error = string.Format("Can't find appropriate organization unit for project with id = {0}", projectId);
                return false;
            }

            var allowedWithdrawalStates = new[] { (int)WithdrawalStatus.Error, (int)WithdrawalStatus.Reverted };

            var withdrawalInfosQuery = _finder.Find(AccountSpecs.Withdrawals.Find.ForPeriod(timePeriod));
            var blockingWithdrawals = _finder.Find(Specs.Find.NotDeleted<Lock>() &&
                                                   AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(organizationUnitId.Value, timePeriod))
                                             .Select(x => x.Order.SourceOrganizationUnit)
                                             .GroupJoin(withdrawalInfosQuery,
                                                        ou => ou.Id,
                                                        wi => wi.OrganizationUnitId,
                                                        (ou, wi) => new
                                                            {
                                                                OrganizationUnit = ou,
                                                                LastWithdrawal = wi.OrderByDescending(x => x.StartDate).FirstOrDefault()
                                                            })
                                             .Where(x => x.LastWithdrawal != null && !allowedWithdrawalStates.Contains(x.LastWithdrawal.Status))
                                             .Select(x => new
                                                 {
                                                     OrgUnitId = x.OrganizationUnit.Id,
                                                     OrgUnitName = x.OrganizationUnit.Name
                                                 })
                                             .ToArray();

            if (blockingWithdrawals.Any())
            {
                error = string.Format("Can't create charges. The following organization units have succeeded or in-progress withdrawal: {0}",
                                      string.Join(", ", blockingWithdrawals.Select(x => string.Format("[{0} - {1}]", x.OrgUnitId, x.OrgUnitName))));
                return false;
            }

            return true;
        }
    }
}