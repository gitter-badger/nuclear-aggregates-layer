using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5883, "Добавляем индекс IX_FirmContacts_SortingPosition_CardId")]
    public sealed class Migration5883 : TransactedMigration
    {
        const string IndexName = "IX_FirmContacts_SortingPosition_CardId";
        protected override void ApplyOverride(IMigrationContext context)
        {

            #region Текст запроса

            const string query = @"CREATE NONCLUSTERED INDEX IX_FirmContacts_SortingPosition_CardId
ON [BusinessDirectory].[FirmContacts] ([SortingPosition],[CardId])
INCLUDE ([Id],[FirmAddressId],[CreatedBy],[CreatedOn])
GO
";

            #endregion

            var table = context.Database.Tables["FirmContacts", ErmSchemas.BusinessDirectory];

            var index = table.Indexes[IndexName];
            if (index != null)
                return;

            context.Connection.ExecuteNonQuery(query);
        }
    }
}
