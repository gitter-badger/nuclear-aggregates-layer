using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified
{
    public interface IReportSimplifiedModel : ISimplifiedModelConsumer
    {
        IEnumerable<ReportDto> GetReports(long userId);
        IEnumerable<ReportFieldDto> GetReportFields(long reportId, long userId);
        bool IsUserFromHeadBranch(long userId);
    }
}
