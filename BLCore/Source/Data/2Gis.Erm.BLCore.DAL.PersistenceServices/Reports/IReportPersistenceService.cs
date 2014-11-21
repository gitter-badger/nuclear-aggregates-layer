using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO;
using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports
{
    public interface IReportPersistenceService : ISimplifiedPersistenceService
    {
        IEnumerable<ReportDto> GetReportNames(long userId);
        IEnumerable<ReportFieldDto> GetReportFields(long reportId, long userId);
    }
}