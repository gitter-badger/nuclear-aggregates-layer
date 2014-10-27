using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25523, "Добавим свежих записей в [Shared].[TimeZones]", "y.baranihin")]
    public class Migration25523 : TransactedMigration
    {
        private readonly string[] timezonesToAdd =
            {
                "Bahia Standard Time",
                "Libya Standard Time",
                "Belarus Standard Time",
                "Russia Time Zone 3",
                "Russia Time Zone 10",
                "Russia Time Zone 11",
                "Line Islands Standard Time"
            };

        private readonly Queue<long> pregeneratedIds = new Queue<long>();


        protected override void ApplyOverride(IMigrationContext context)
        {
            pregeneratedIds.Enqueue(478646265898455240);
            pregeneratedIds.Enqueue(478650165907061192);
            pregeneratedIds.Enqueue(478650499362744520);
            pregeneratedIds.Enqueue(478650567394383816);
            pregeneratedIds.Enqueue(478650620871780552);
            pregeneratedIds.Enqueue(478650669232141000);
            pregeneratedIds.Enqueue(478650718783682248);

            const string QueryTemplate = @"if not exists(select * from [Shared].[TimeZones] where TimeZoneId = '{0}') 
                                            insert into [Shared].[TimeZones] (Id, TimeZoneId, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
                                                values ({1}, '{0}', 1, getutcdate(), 1, getutcdate())";

            foreach (var timezoneToAdd in timezonesToAdd)
            {
                context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, timezoneToAdd, pregeneratedIds.Dequeue()));
            }
        }
    }
}