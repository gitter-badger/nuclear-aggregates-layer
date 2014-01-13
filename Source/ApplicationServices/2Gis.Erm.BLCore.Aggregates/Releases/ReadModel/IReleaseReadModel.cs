using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Releases.DTO;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel
{
    public interface IReleaseReadModel : IAggregateReadModel<ReleaseInfo>
    {
        bool IsReleaseMustBeLaunchedThroughExport(long organizationUnitId);
        long GetOrganizationUnitId(int organizationUnitDgppId);
        string GetOrganizationUnitName(long organizationUnitId);
        ReleaseInfo GetLastRelease(long organizationUnitId, TimePeriod period);
        bool HasFinalReleaseInProgress(long organizationUnitId, TimePeriod period);
        bool HasFinalReleaseAfterDate(long organizationUnitId, DateTime periodStartDate);
        bool HasSuccededFinalReleaseFromDate(long organizationUnitId, DateTime periodStartDate);
        ReleaseInfo GetReleaseInfo(long releaseInfoId);
        IEnumerable<ReleaseProcessingMessage> GetReleaseValidationResults(long releaseInfoId);
        Dictionary<long, ValidationReportLine> GetOrderValidationLines(IEnumerable<long> orderIds);
    }
}