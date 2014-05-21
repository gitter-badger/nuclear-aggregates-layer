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
            error = null;
            var orgUnitId = _finder.Find(Specs.Find.ById<Project>(projectId)).Select(x => x.OrganizationUnitId).SingleOrDefault();
            if (orgUnitId == null)
            {
                error = string.Format("Can't find organization unit for porject with code = {0}", projectId);
                return false;
            }

            var withdrawalInfos = _finder.Find(AccountSpecs.Withdrawals.Find.ForPeriod(timePeriod) &&
                                               AccountSpecs.Withdrawals.Find.ExceptStates(WithdrawalStatus.Error, WithdrawalStatus.Reverted))
                                         .Select(x => new { OrgUnitId = x.OrganizationUnitId, OrgUnitName = x.OrganizationUnit.Name })
                                         .Distinct()
                                         .ToArray();

            if (withdrawalInfos.Any())
            {
                error = string.Format("Can't create charges. The following organization units have secceeded or in-progress withdrawal: {0}",
                                      string.Join(", ", withdrawalInfos.Select(x => string.Format("[{0} - {1}]", x.OrgUnitId, x.OrgUnitName))));
                return false;
            }

            return true;
        }
    }
}