using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4606, "Удаляем OwnerCode и другие служебные колонки из Shared.MessageTypes")]
    public sealed class Migration4606 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.Tables["MessageTypes", ErmSchemas.Shared];

            var column = table.Columns["IsDeleted"];
            if (column != null)
                column.Drop();

            column = table.Columns["OwnerCode"];
            if (column != null)
                column.Drop();

            column = table.Columns["CreatedBy"];
            if (column != null)
                column.Drop();

            column = table.Columns["ModifiedBy"];
            if (column != null)
                column.Drop();

            column = table.Columns["CreatedOn"];
            if (column != null)
                column.Drop();

            column = table.Columns["ModifiedOn"];
            if (column != null)
                column.Drop();

            column = table.Columns["Timestamp"];
            if (column != null)
                column.Drop();

        }
    }
}
