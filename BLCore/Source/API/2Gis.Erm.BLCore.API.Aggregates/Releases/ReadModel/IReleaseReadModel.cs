using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.DTO;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel
{
    public interface IReleaseReadModel : IAggregateReadModel<ReleaseInfo>
    {
        bool IsReleaseMustBeLaunchedThroughExport(long organizationUnitId);
        long GetOrganizationUnitId(int organizationUnitDgppId);
        string GetOrganizationUnitName(long organizationUnitId);
        ReleaseInfo GetLastFinalRelease(long organizationUnitId, TimePeriod period);
        IReadOnlyCollection<ReleaseInfo> GetReleasesInDescOrder(long organizationUnitId, TimePeriod period);
        bool HasFinalReleaseInProgress(long organizationUnitId, TimePeriod period);
        bool HasFinalReleaseAfterDate(long organizationUnitId, DateTime periodStartDate);
        bool HasSuccededFinalReleaseFromDate(long organizationUnitId, DateTime periodStartDate);
        ReleaseInfo GetReleaseInfo(long releaseInfoId);
        IEnumerable<ReleaseProcessingMessage> GetReleaseValidationResults(long releaseInfoId);
        Dictionary<long, ValidationReportLine> GetOrderValidationLines(IEnumerable<long> orderIds);
        int GetCountryCode(long organizationUnitId);
    }
}