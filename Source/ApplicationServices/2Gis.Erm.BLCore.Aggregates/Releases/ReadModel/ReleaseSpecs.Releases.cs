﻿using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel
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

                public static FindSpecification<ReleaseInfo> FinalForPeriodWithStatus(TimePeriod period, ReleaseStatus status)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.IsActive
                                                                   && !x.IsDeleted
                                                                   && x.PeriodStartDate == period.Start
                                                                   && x.PeriodEndDate == period.End
                                                                   && !x.IsBeta
                                                                   && x.Status == (int)status);
                }

                public static FindSpecification<ReleaseInfo> FinalInProgress(long organizationUnitId, TimePeriod period)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.IsActive && !x.IsDeleted
                                                                   && !x.IsBeta
                                                                   && x.PeriodStartDate >= period.Start && x.PeriodEndDate <= period.End
                                                                   && x.OrganizationUnitId == organizationUnitId
                                                                   && (x.Status == (int)ReleaseStatus.InProgressInternalProcessingStarted
                                                                       || x.Status == (int)ReleaseStatus.InProgressWaitingExternalProcessing));
                }

                public static FindSpecification<ReleaseInfo> FinalSuccessOrInProgressAfterDate(long organizationUnitId, DateTime periodStartDate)
                {
                    return new FindSpecification<ReleaseInfo>(x => x.IsActive && !x.IsDeleted &&
                                                                   !x.IsBeta &&
                                                                   (x.Status == (int)ReleaseStatus.InProgressInternalProcessingStarted
                                                                    || x.Status == (int)ReleaseStatus.InProgressWaitingExternalProcessing
                                                                    || x.Status == (int)ReleaseStatus.Success) &&
                                                                   x.OrganizationUnitId == organizationUnitId &&
                                                                   x.PeriodStartDate > periodStartDate);
                }
            }
        }
    }
}
