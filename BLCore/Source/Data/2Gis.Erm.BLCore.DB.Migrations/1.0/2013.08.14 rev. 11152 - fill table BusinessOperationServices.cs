using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // После первого апгрейда в ветке 0.18 были добавлены ещё несколько операций, эта миграция удаляет записи,
    // добавленные с использованием перечисления OperationName и создаёт их аналоги с использованием XIdentity
    [Migration(11152, "Заполнение BusnessOperationServices")]
    public class Migration11152 : TransactedMigration
    {
        private const string DeleteStatements = @"
delete from Shared.BusinessOperationServices where Descriptor = 1186303282 and Operation = 4 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 1186303282 and Operation = 1 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 1186303282 and Operation = 8 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 1186303282 and Operation = 3 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 653315513 and Operation = 1 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 653315513 and Operation = 8 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 653315513 and Operation = 112 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 653315513 and Operation = 3 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 1797543439 and Operation = 1 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 942141479 and Operation = 7 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 942141479 and Operation = 1 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 942141479 and Operation = 6 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = 942141479 and Operation = 2 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = -394024283 and Operation = 10 and Service = 11
delete from Shared.BusinessOperationServices where Descriptor = -918089089 and Operation = 6 and Service = 11
go
";

        private const string InsertIfNotExists = @"
if not exists(select count(*) from Shared.BusinessOperationServices where Descriptor = {0} and Operation = {1} and Service = {2})
    insert into Shared.BusinessOperationServices(Descriptor, Operation, Service) values ({0}, {1}, {2})
";

        private static readonly Tuple<int, int, int>[] InsertValues = new[]
            {
                new Tuple<int, int, int>(1186303282, 4, 11),
                new Tuple<int, int, int>(1186303282, 30, 11),
                new Tuple<int, int, int>(1186303282, 31, 11),
                new Tuple<int, int, int>(1186303282, 10, 11),
                new Tuple<int, int, int>(1186303282, 16, 11),
                new Tuple<int, int, int>(653315513, 30, 11),
                new Tuple<int, int, int>(653315513, 31, 11),
                new Tuple<int, int, int>(653315513, 10, 11),
                new Tuple<int, int, int>(653315513, 14602, 11),
                new Tuple<int, int, int>(653315513, 16, 11),
                new Tuple<int, int, int>(1797543439, 30, 11),
                new Tuple<int, int, int>(1797543439, 31, 11),
                new Tuple<int, int, int>(942141479, 2, 11),
                new Tuple<int, int, int>(942141479, 30, 11),
                new Tuple<int, int, int>(942141479, 31, 11),
                new Tuple<int, int, int>(942141479, 8, 11),
                new Tuple<int, int, int>(942141479, 9, 11),
                new Tuple<int, int, int>(-394024283, 25, 11),
                new Tuple<int, int, int>(-918089089, 8, 11),
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(DeleteStatements);

            foreach (var insertValue in InsertValues)
            {
                context.Connection.ExecuteNonQuery(string.Format(InsertIfNotExists, insertValue.Item1, insertValue.Item2, insertValue.Item3));
            }
        }
    }
}
