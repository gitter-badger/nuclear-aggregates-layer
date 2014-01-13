using System;
using System.IO;

using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports
{
    public interface IPlanningReportPersistenceService : ISimplifiedPersistenceService
    {
        Stream GetLegalPersonPaymentsStream(long organizationUnitId, DateTime reportDate, long currentUser, bool considerOnRegistration, bool considerOnApproval);
        Stream GetPlanningReportStream(long organizationUnitId, DateTime planningMonth, bool isAdvertisingAgency);
    }
}
