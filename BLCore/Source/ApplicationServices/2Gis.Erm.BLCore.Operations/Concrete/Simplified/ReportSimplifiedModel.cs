using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    public sealed class ReportSimplifiedModel : IReportSimplifiedModel
    {
        private readonly IReportPersistenceService _reportPersistenceService;
        private readonly IFinder _finder;

        public ReportSimplifiedModel(IFinder finder, IReportPersistenceService reportPersistenceService)
        {
            _finder = finder;
            _reportPersistenceService = reportPersistenceService;
        }

        public bool IsUserFromHeadBranch(long userId)
        {
            var user = _finder.FindObsolete(Specs.Find.ById<User>(userId)).Single();
            return user.DepartmentId == 1;
        }

        public IEnumerable<ReportDto> GetReports(long userId)
        {
            return _reportPersistenceService.GetReportNames(userId)
                                            .Where(dto => !dto.IsHidden);
        }

        public IEnumerable<ReportFieldDto> GetReportFields(long reportId, long userId)
        {
            try
            {
                return _reportPersistenceService.GetReportFields(reportId, userId)
                .OrderBy(dto => dto.DisplayOrder)
                .ToList();
            }
            catch (Exception innerException)
            {
                var message = string.Format(BLResources.ReportUIGenerationError, reportId);
                throw new ArgumentException(message, innerException);
            }
        }
    }
}
