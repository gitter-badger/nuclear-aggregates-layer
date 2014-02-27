using System;
using System.Data.SqlClient;
using System.IO;

namespace DoubleGis.Erm.BL.Reports
{
    public class ReportsSqlConnectionWrapper : IReportsSqlConnectionWrapper
    {
        private readonly string _connectionString;

        public ReportsSqlConnectionWrapper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Stream GetLegalPersonPaymentsStream(long organizationUnitId, DateTime reportDate, long currentUser, bool considerOnRegistration, bool considerOnApproval)
        {
            using(var sqlConnection = new SqlConnection(_connectionString))
            {
                var legalPersonPayments = new LegalPersonPayments.LegalPersonPayments(sqlConnection,
                                                                                      organizationUnitId,
                                                                                      reportDate,
                                                                                      currentUser,
                                                                                      considerOnRegistration,
                                                                                      considerOnApproval);
                return legalPersonPayments.ExecuteStream();
            }
        }

        public Stream GetPlanningReportStream(long organizationUnitId, DateTime planningMonth, bool isAdvertisingAgency)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                var planningReport = new PlanningReport.PlanningReport(sqlConnection, organizationUnitId, planningMonth, isAdvertisingAgency);
                return planningReport.ExecuteStream();
            }
        }
    }
}