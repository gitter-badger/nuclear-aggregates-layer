using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6965, "Изменяем длины колонок в таблицах Client, Contact")]
    public sealed class Migration6965 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
            -- Clients
            ALTER TABLE Billing.Clients ALTER COLUMN Name nvarchar(250)
            ALTER TABLE Billing.Clients ALTER COLUMN Website nvarchar(200)
            ALTER TABLE Billing.Clients ALTER COLUMN Email nvarchar(100)
            ALTER TABLE Billing.Clients ALTER COLUMN Fax nvarchar(50)

            -- Contacts
            ALTER TABLE Billing.Contacts ALTER COLUMN WorkEmail nvarchar(100)
            ALTER TABLE Billing.Contacts ALTER COLUMN HomeEmail nvarchar(100)
            ALTER TABLE Billing.Contacts ALTER COLUMN Website nvarchar(200)
            ALTER TABLE Billing.Contacts ALTER COLUMN Fax nvarchar(50)
            ALTER TABLE Billing.Contacts ALTER COLUMN Department nvarchar(100)
            ");
        }
    }
}
