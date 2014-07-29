using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(9124, "Добавление записей BusnessOperationServices")]
    public class Migration9124 : TransactedMigration
    {
        private const string InsertStatements = @"
delete from [Shared].[BusinessOperationServices] where [Descriptor] = 1186303282 and [Operation] = 4 and [Service] = 1
delete from [Shared].[BusinessOperationServices] where [Descriptor] = 942141479 and [Operation] = 4 and [Service] = 1
go

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-566764810, 10, 1, N'floworders.order;flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (-394024283, 10, 1, N'floworders.order;flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 11, 1, N'floworders.order;flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (942141479, 4, 1, N'floworders.order;flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (936439653, 1, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (936439653, 2, 1, N'flowfinancialdata.legalentity')
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service], [ServiceParameter]) VALUES (1186303282, 4, 1, N'floworders.order;flowfinancialdata.legalentity')
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
