using System;
using System.Data.SqlClient;
using System.IO;

using DoubleGis.Erm.BLCore.Reports.LegalPersonPayments;
using DoubleGis.Erm.BLCore.Reports.PlanningReport;
using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports
{
    public class PlanningReportPersistenceService : IPlanningReportPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public PlanningReportPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public Stream GetLegalPersonPaymentsStream(long organizationUnitId, DateTime reportDate, long currentUser, bool considerOnRegistration, bool considerOnApproval)
        {
            return _databaseCaller.ExecuteInConnection(dbConnection =>
            {
                // assume that our connection is connection to microsoft sql server
                var sqlConnection = (SqlConnection)dbConnection;

                var legalPersonPayments = new LegalPersonPayments(sqlConnection,
                                                                  organizationUnitId,
                                                                  reportDate,
                                                                  currentUser,
                                                                  considerOnRegistration,
                                                                  considerOnApproval);
                return legalPersonPayments.ExecuteStream();
            });
        }

        public Stream GetPlanningReportStream(long organizationUnitId, DateTime planningMonth, bool isAdvertisingAgency)
        {
            return _databaseCaller.ExecuteInConnection(dbConnection =>
            {
                // assume that our connection is connection to microsoft sql server
                var sqlConnection = (SqlConnection)dbConnection;
                var planningReport = new PlanningReport(sqlConnection, organizationUnitId, planningMonth, isAdvertisingAgency); 
                return planningReport.ExecuteStream();
            });
        }
    }
}