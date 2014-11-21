using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(23836, "Добавление привелегий для связей фирм и юр.юлюц со сделкой", "y.baranihin")]
    public class Migration23836 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int ReadOperation = 1;
            const int FirmDealEntity = 610;
            const int LegalPersonDealEntity = 612;

            const long Id1 = 441735163953330376;
            const long Id2 = 441735326887606472;

            InsertPrivelege(Id1, FirmDealEntity, ReadOperation, context.Connection);
            InsertPrivelege(Id2, LegalPersonDealEntity, ReadOperation, context.Connection);
        }

        protected void InsertPrivelege(long privelegeId, int entityType, int operation, ServerConnection connection)
        {
            const string Query = @"if not exists(select * from [Security].[Privileges] where EntityType = {1} and Operation = {2})
                                   insert into [Security].[Privileges] (Id, EntityType, Operation) values ({0}, {1}, {2})";

            connection.ExecuteNonQuery(string.Format(Query, privelegeId, entityType, operation));
        }
    }
}