using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7262, "Заведение индекса для логa CRM.")]
    public sealed class Migration7262 : TransactedMigration, INonDefaultDatabaseMigration
    {
        public ErmConnectionStringKey ConnectionStringKey 
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"CREATE NONCLUSTERED INDEX CRM_WorkflowLog_AsyncOperationID ON [dbo].[WorkflowLogBase] ([AsyncOperationID])
GO ");
        }
    }
}
