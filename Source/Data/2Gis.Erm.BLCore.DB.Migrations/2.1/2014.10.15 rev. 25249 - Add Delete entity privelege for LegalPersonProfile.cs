using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25249, "Разрешение на удаление профилей", "a.rechkalov")]
    public class Migration25249 : TransactedMigration
    {
        private const int DeleteOperation = 65536;

        private const int LegalPersonProfile = 219;

        private const long DeletePrivelegeId = 472673923880750849;

        protected override void ApplyOverride(IMigrationContext context)
        {
            InsertPrivelege(DeletePrivelegeId, LegalPersonProfile, DeleteOperation, context.Connection);
        }

        protected void InsertPrivelege(long privelegeId, int entityType, int operation, ServerConnection connection)
        {
            const string Query = @"if not exists(select * from [Security].[Privileges] where EntityType = {1} and Operation = {2})
                                   insert into [Security].[Privileges] (Id, EntityType, Operation) values ({0}, {1}, {2})";

            connection.ExecuteNonQuery(string.Format(Query, privelegeId, entityType, operation));
        }
    }
}