using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23946, "Добавление привелегии деактивации юр. лиц", "y.baranihin")]
    public class Migration23946 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int DeactivateLegalPersonOperation = 650;

            const long PrivilegeId = 442462396672151240;
            const long DepthId = 442480078667090632;
            

            const string Query = @"if not exists(select * from [Security].[Privileges] where EntityType is NULL and Operation = {1})
                                   begin 
                                    insert into [Security].[Privileges] (Id, Operation) values ({0}, {1})
                                    insert into [Security].[FunctionalPrivilegeDepths] (Id, PrivilegeId, LocalResourceName, Priority, Mask) values ({2}, {0}, 'FPrvDpthGranted', 1, 134)
                                   end ";

            context.Connection.ExecuteNonQuery(string.Format(Query, PrivilegeId, DeactivateLegalPersonOperation, DepthId));
        }
    }
}