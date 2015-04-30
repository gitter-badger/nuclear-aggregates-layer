using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core;
using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel
{
    public static class ReleaseSpecs
    {
        public static class Releases
        {
            public static class Find
            {
                public static FindSpecification<ReleaseInfo> ByOrganization(long organizationUnitId)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<ReleaseInfo> ForPeriod(TimePeriod period)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End);
                }

                public static FindSpecification<ReleaseInfo> Final()
                {
                    return new FindSpecification<ReleaseInfo>(x => !x.IsBeta);
                }

                public static FindSpecification<ReleaseInfo> Succeeded()
                {
                    return new FindSpecification<ReleaseInfo>(x => x.Status == ReleaseStatus.Success);
                }

                public static FindSpecification<ReleaseInfo> FinalForPeriodWithStatuses(TimePeriod period, params ReleaseStatus[] statuses)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.IsActive
                                                                   && !x.IsDeleted
                                                                   && x.PeriodStartDate == period.Start
                                                                   && x.PeriodEndDate == period.End
                                                                   && !x.IsBeta
                                                                   && statuses.Contains(x.Status));
                }

                public static FindSpecification<ReleaseInfo> FinalInProgress(long organizationUnitId, TimePeriod period)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.IsActive && !x.IsDeleted
                                                                   && !x.IsBeta
                                                                   && x.PeriodStartDate >= period.Start && x.PeriodEndDate <= period.End
                                                                   && x.OrganizationUnitId == organizationUnitId
                                                                   && (x.Status == ReleaseStatus.InProgressInternalProcessingStarted
                                                                       || x.Status == ReleaseStatus.InProgressWaitingExternalProcessing));
                }

                public static FindSpecification<ReleaseInfo> FinalSuccessOrInProgressAfterDate(long organizationUnitId, DateTime periodStartDate)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.IsActive && !x.IsDeleted &&
                                                                   !x.IsBeta &&
                                                                   (x.Status == ReleaseStatus.InProgressInternalProcessingStarted
                                                                    || x.Status == ReleaseStatus.InProgressWaitingExternalProcessing
                                                                    || x.Status == ReleaseStatus.Success) &&
                                                                   x.OrganizationUnitId == organizationUnitId &&
                                                                   x.StartDate > periodStartDate);
                }
            }
        }
    }
}