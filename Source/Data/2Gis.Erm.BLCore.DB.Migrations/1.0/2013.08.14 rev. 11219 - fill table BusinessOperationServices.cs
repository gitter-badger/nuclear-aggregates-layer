using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(11219, "Операция CreateOrUpdate разделилась на две: Create & Update")]
    public class Migration11219 : TransactedMigration
    {
        private const string InsertStatements = @"
insert into [Shared].[BusinessOperationServices](Descriptor, Operation, Service, ServiceParameter)
select Descriptor, 30, Service, ServiceParameter
from [Shared].[BusinessOperationServices]
where [BusinessOperationServices].Operation = 23

insert into [Shared].[BusinessOperationServices](Descriptor, Operation, Service, ServiceParameter)
select Descriptor, 31, Service, ServiceParameter
from [Shared].[BusinessOperationServices]
where [BusinessOperationServices].Operation = 23

delete from [Shared].[BusinessOperationServices]
where [BusinessOperationServices].Operation = 23
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(InsertStatements);
        }
    }
}
