using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6470, "Добавление колонок в таблицу FirmsForPostIntegrationActivities")]
    public class Migration6470 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            #region Текст запроса
            const string Query = @"
Alter Table [Integration].[FirmsForPostIntegrationActivities] ADD ExportOrders Bit not null Default 0;
Alter Table [Integration].[FirmsForPostIntegrationActivities] ADD ReplicateObjects Bit not null Default 0";
            #endregion

            context.Connection.ExecuteNonQuery(Query);
        }
    }
}