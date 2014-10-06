using System;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions;
using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Metadata.Crm;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.CRM;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Maintenance
{
    [Migration(23613, "Cleaning up system activities in MS CRM.", "s.pomadin")]
    public class CrmSystemActivitiesCleanup : IContextedMigration<ICrmMigrationContext>
    {
        private static readonly TimeSpan Timeout = new TimeSpan(0, 30, 0);      // 30 minutes
        private static readonly TimeSpan SleepTime = new TimeSpan(0, 0, 5);     // 5 seconds

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmConnection; }
        }

        public void Revert(ICrmMigrationContext context)
        {
            throw new NotImplementedException();
        }

        public void Apply(ICrmMigrationContext context)
        {
            using (var service = context.CrmContext.CreateService())
            {
                var jobId = EnqueueBulkDeleteOperation(service);

                if (!MonitorBulkDeleteOperation(service, jobId))
                {
                    throw new Exception("The CRM activity was not deleted.");
                }
            }
        }

        private static Guid EnqueueBulkDeleteOperation(ICrmService service)
        {
            var bulkDeleteRequest = new BulkDeleteRequest
                {
                    JobName = "Activity Bulk Delete Job",
                    QuerySet = new QueryBase[]
                        {
                            BuildQuery(EntityName.serviceappointment), // 4214 - действие сервиса
                            BuildQuery(EntityName.campaignresponse), // 4401 - контракт от кампании
                        },
                    StartDateTime = CrmDateTime.Now,
                    RecurrencePattern = String.Empty,
                    SendEmailNotification = false,
                    ToRecipients = new Guid[] { },
                    CCRecipients = new Guid[] { }
                };

            var response = (BulkDeleteResponse) service.Execute(bulkDeleteRequest);
            return response.JobId;
        }

        private static bool MonitorBulkDeleteOperation(ICrmService service, Guid jobId)
        {
            // query for bulk delete operation and check for status
            var bulkQuery = new QueryByAttribute
            {
                EntityName = EntityName.bulkdeleteoperation.ToString(),
                ColumnSet = new ColumnSet(new[]
                    {
                        BulkDeleteOperation.StateCode,
                        BulkDeleteOperation.StatusCode,
                    }),
                Attributes = new [] { BulkDeleteOperation.AsyncOperationId },
                Values = new object[] { jobId }
            };

            var startTime = DateTime.Now;
            do
            {
                var entityCollection = service.RetrieveMultiple(bulkQuery);
                
                var bulkDeleteOperation = entityCollection.BusinessEntities.Cast<DynamicEntity>().FirstOrDefault();
                if (bulkDeleteOperation == null)
                {
                    continue;
                }

                var stateCode = Convert.ToString(bulkDeleteOperation.Value(BulkDeleteOperation.StateCode));
                if (stateCode == "Completed")
                {
                    var statusCode = bulkDeleteOperation.Value(BulkDeleteOperation.StatusCode) as Status;
                    return statusCode != null && statusCode.Value == 30; // Succeeded
                }

                Thread.Sleep(SleepTime);
            }
            while ((DateTime.Now - startTime) < Timeout);

            // timeout
            return false;
        }

        private static QueryExpression BuildQuery(EntityName entityName)
        {
            return new QueryExpression
            {
                EntityName = entityName.ToString(),
            };
        }
    }
}
