using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12098, "Заполнение BusnessOperationServices")]
    public class Migration12098 : TransactedMigration
    {
        private const string InsertStatements = @"
-- 'Смена реквизитов юр. лица' в поток ExportFlowFinancialDataLegalEntity
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 14701, 3)

-- ActivateIdentity для Юрлица в поток flowFinancialData.Client
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (942141479, 2, 11)

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
