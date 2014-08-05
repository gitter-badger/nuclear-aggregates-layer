
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(21623, "Добавление привелегий AdvertisementElementStatus, AdvertisementElementDenialReason", "a.rechkalov")]
    public class Migration21623 : TransactedMigration
    {
        const int ReadOperation = 1;
        const int UpdateOperation = 2;
        const int CreateOperation = 32;
        const int DeleteOperation = 65536;

        const int AdvertisementElementDenialReason = 315;
        const int AdvertisementElementStatus = 316;

        private static readonly Queue<long> Ids = new Queue<long>(new[]
            {
                391667667586515201,
                391667720015315457,
                391667762277122817,
                391667812591993857,
                391667859777914113,
                391667922323375617,
                391667969375078145,
                391668026426001409,
            });

        protected override void ApplyOverride(IMigrationContext context)
        {
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementDenialReason, ReadOperation, context.Connection);
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementDenialReason, UpdateOperation, context.Connection);
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementDenialReason, CreateOperation, context.Connection);
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementDenialReason, DeleteOperation, context.Connection);

            InsertPrivelege(Ids.Dequeue(), AdvertisementElementStatus, ReadOperation, context.Connection);
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementStatus, UpdateOperation, context.Connection);
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementStatus, CreateOperation, context.Connection);
            InsertPrivelege(Ids.Dequeue(), AdvertisementElementStatus, DeleteOperation, context.Connection);
        }

        protected void InsertPrivelege(long privelegeId, int entityType, int operation, ServerConnection connection)
        {
            const string Query = @"if not exists(select * from [Security].[Privileges] where EntityType = {1} and Operation = {2})
                                   insert into [Security].[Privileges] (Id, EntityType, Operation) values ({0}, {1}, {2})";

            connection.ExecuteNonQuery(string.Format(Query, privelegeId, entityType, operation));
        }
    }
}