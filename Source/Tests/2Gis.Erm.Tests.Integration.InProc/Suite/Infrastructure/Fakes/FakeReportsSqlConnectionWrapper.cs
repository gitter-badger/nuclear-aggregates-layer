using System;
using System.IO;
using System.Text;

using DoubleGis.Erm.BL.Reports;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes
{
    class FakeReportsSqlConnectionWrapper : IReportsSqlConnectionWrapper
    {
        public Stream GetLegalPersonPaymentsStream(long organizationUnitId, DateTime reportDate, long currentUser, bool considerOnRegistration, bool considerOnApproval)
        {
            var inputParameters = string.Format("{0};{1};{2};{3};{4}",
                                                organizationUnitId,
                                                reportDate,
                                                currentUser,
                                                considerOnRegistration,
                                                considerOnApproval);

            return new MemoryStream(Encoding.Default.GetBytes(inputParameters));
        }

        public Stream GetPlanningReportStream(long organizationUnitId, DateTime planningMonth, bool isAdvertisingAgency)
        {
            var inputParameters = string.Format("{0};{1};{2}",
                                                organizationUnitId,
                                                planningMonth,
                                                isAdvertisingAgency);

            return new MemoryStream(Encoding.Default.GetBytes(inputParameters));
        }
    }
}
