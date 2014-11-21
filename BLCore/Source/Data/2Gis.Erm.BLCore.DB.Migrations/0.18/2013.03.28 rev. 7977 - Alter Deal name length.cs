using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7977, "Уменьшаем длину поля Имя в Сделке до 300 символов (баг 3849)")]
    public sealed class Migration7977 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
UPDATE Billing.Deals SET Name = SUBSTRING(Name, 1, 300)

DROP INDEX [IX_Deals_ClientId_IsDeleted_IsActive] ON [Billing].[Deals]

ALTER TABLE Billing.Deals ALTER COLUMN Name NVARCHAR(300) NOT NULL

CREATE NONCLUSTERED INDEX [IX_Deals_ClientId_IsDeleted_IsActive] ON [Billing].[Deals]
(
	[ClientId] ASC,
	[IsDeleted] ASC,
	[IsActive] ASC
)
INCLUDE ( 	[Id],
	[Name],
	[MainFirmId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
");
        }
    }
}