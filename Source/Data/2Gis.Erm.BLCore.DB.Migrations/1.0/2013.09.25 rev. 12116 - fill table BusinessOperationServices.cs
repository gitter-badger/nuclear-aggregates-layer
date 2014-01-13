using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(12116, "Заполнение BusnessOperationServices")]
    public class Migration12116 : TransactedMigration
    {
        private const string InsertStatements = @"
-- Добавление NonCoupled операций с нулевым дескриптором согласно текущим метаданным

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 18601, 4)

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 21901, 3)

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 15105, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 15106, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 15107, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 15108, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 15109, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 19801, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 32, 5)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (0, 33, 5)

-- Добавление нового варианта операции Upload (временно в BusinessOperationServices присутствуют оба)

INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1805571956, 12, 4)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1805571956, 12, 6)
INSERT INTO [Shared].[BusinessOperationServices] ([Descriptor], [Operation], [Service]) VALUES (-1805571956, 12, 7)

-- Исправляю ошибку миграции 11811: была использована 23 - CreateOrUpdateIdentity вместо 25 - MergeIdentity
-- и позже в миграции 12066 добавлена операция 25 без удаления 23

DELETE FROM [Shared].[BusinessOperationServices] WHERE [Descriptor] = 1186303282 and [Operation] = 23 and [Service] = 11

go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
