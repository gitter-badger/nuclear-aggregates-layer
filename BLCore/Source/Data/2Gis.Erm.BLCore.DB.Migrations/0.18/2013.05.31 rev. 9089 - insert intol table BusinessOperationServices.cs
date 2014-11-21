using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9089, "Добавление записей BusnessOperationServices")]
    public class Migration9089 : TransactedMigration
    {
        private const string InsertStatements = @"
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 109, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 9, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-2081716307, 4, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (1186303282, 4, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-894528218, 4, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (1905845000, 2, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (1186303282, 8, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (1186303282, 3, 1, N'floworders.order')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 4, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (936439653, 4, 1, N'flowfinancialdata.legalentity')
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
