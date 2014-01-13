using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7700, "Удаление из CRM представлений, связанных с отчётами и самих отчётов")]
    public sealed class Migration7700 : TransactedMigration, INonDefaultDatabaseMigration
    {
        private const string DeleteReports = 
            @"update [dbo].ReportBase set DeletionStateCode = 1 where DeletionStateCode = 0";

        private const string DeleteUserReportQueries =
            @"update [dbo].[UserQueryBase] set DeletionStateCode = 1 where DeletionStateCode = 0 and ReturnedTypeCode = 9100";

        public ErmConnectionStringKey ConnectionStringKey 
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(DeleteReports);
            context.Connection.ExecuteNonQuery(DeleteUserReportQueries);
        }
    }
}
