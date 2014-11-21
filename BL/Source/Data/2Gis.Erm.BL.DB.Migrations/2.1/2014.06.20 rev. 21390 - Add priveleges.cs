using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21390, "Добавление привелегий DenialReasons", "y.baranihin")]
    public class Migration21390 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int ReadOperation = 1;
            const int UpdateOperation = 2;
            const int CreateOperation = 32;
            const int DeleteOperation = 65536;
            const int DenialReasonEntity = 313;

            const long Id1 = 390126721564667336;
            const long Id2 = 390126849491289800;
            const long Id3 = 390126989732202440;
            const long Id4 = 390131995462587080;

            InsertPrivelege(Id1, DenialReasonEntity, ReadOperation, context.Connection);
            InsertPrivelege(Id2, DenialReasonEntity, UpdateOperation, context.Connection);
            InsertPrivelege(Id3, DenialReasonEntity, CreateOperation, context.Connection);
            InsertPrivelege(Id4, DenialReasonEntity, DeleteOperation, context.Connection);
        }

        protected void InsertPrivelege(long privelegeId, int entityType, int operation, ServerConnection connection)
        {
            const string Query = @"if not exists(select * from [Security].[Privileges] where EntityType = {1} and Operation = {2})
                                   insert into [Security].[Privileges] (Id, EntityType, Operation) values ({0}, {1}, {2})";

            connection.ExecuteNonQuery(string.Format(Query, privelegeId, entityType, operation));
        }
    }
}